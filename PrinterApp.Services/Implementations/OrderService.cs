using Microsoft.AspNetCore.Http;
using PrinterApp.Data.UnitOfWork;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;
using System.Collections.Generic;

namespace PrinterApp.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUploadService _fileUploadService;
        private readonly string[] _allowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".doc", ".docx", ".xls", ".xlsx", ".bmp" };
        private readonly long _maxFileSize = 10 * 1024 * 1024; // 10 MB

        public OrderService(IUnitOfWork unitOfWork, IFileUploadService fileUploadService)
        {
            _unitOfWork = unitOfWork;
            _fileUploadService = fileUploadService;
        }

        // ===== العمليات الأساسية =====

        

        public async Task<IEnumerable<OrderViewModel>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();
            return orders.Select(MapToViewModel);
        }

        public async Task<IEnumerable<OrderViewModel>> GetActiveOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.FindAsync(o => o.IsActive);
            return orders.OrderByDescending(o => o.CreatedDate).Select(MapToViewModel);
        }

        public async Task<OrderViewModel> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            return order != null ? MapToViewModel(order) : null;
        }

        public async Task<OrderViewModel> GetOrderByNumberAsync(string orderNumber)
        {
            var order = await _unitOfWork.Orders.GetByOrderNumberAsync(orderNumber);
            return order != null ? MapToViewModel(order) : null;
        }

        public async Task<OrderDetailsViewModel> GetOrderWithAllDetailsAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetWithAllDetailsAsync(id);
            if (order == null)
                return null;

            return new OrderDetailsViewModel
            {
                Order = MapToViewModel(order),
                Attachments = order.Attachments?.Select(a => MapAttachmentToViewModel(a)).ToList() ?? new List<OrderAttachmentViewModel>(),
                ManufacturingItems = order.ManufacturingItems?.Select(mi => MapManufacturingItemToViewModel(mi)).ToList() ?? new List<ManufacturingItemViewModel>(),
                Timeline = order.Timeline?.OrderByDescending(t => t.ActionDate).Select(t => MapTimelineToViewModel(t)).ToList() ?? new List<OrderTimelineViewModel>()
            };
        }

        public async Task<bool> OrderNumberExistsAsync(string orderNumber, int? excludeId = null)
        {
            return await _unitOfWork.Orders.OrderNumberExistsAsync(orderNumber, excludeId);
        }

        // ===== مرحلة الطلب (Stage 1) =====

        public async Task<(bool Success, string[] Errors)> CreateOrderStage1Async(OrderStage1ViewModel model, IFormFileCollection files, string userId, string userName)
        {
            var errors = new List<string>();

            if (await OrderNumberExistsAsync(model.OrderNumber))
            {
                errors.Add("رقم الطلب موجود مسبقاً");
                return (false, errors.ToArray());
            }

            try
            {
                var order = new Order
                {
                    OrderNumber = model.OrderNumber,
                    OrderDate = model.OrderDate,
                    ExpectedDeliveryDate = model.ExpectedDeliveryDate,
                    CustomerId = model.CustomerId,
                    SupplierId = model.SupplierId,
                    ProductId = model.ProductId,
                    RollDirectionId = model.RollDirectionId,
                    RawMaterialId = model.RawMaterialId,
                    Length = model.Length,
                    Width = model.Width,
                    Quantity = model.Quantity,
                    OrderNotes = model.OrderNotes,
                    Stage = OrderStage.Order,
                    Status = OrderStatus.Pending,
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    CreatedBy = userId
                };

                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.CompleteAsync();

                // رفع الملفات
                if (files != null && files.Count > 0)
                {
                    await UploadOrderFilesAsync(order.Id, files, userId, errors);
                }

                // إضافة Timeline Entry
                await _unitOfWork.OrderTimelines.AddTimelineEntryAsync(
                    order.Id,
                    OrderStage.Order,
                    OrderStatus.Pending,
                    "تم إنشاء الطلب",
                    model.OrderNotes,
                    userId,
                    userName
                );
                await _unitOfWork.CompleteAsync();

                return (true, errors.ToArray());
            }
            catch (Exception ex)
            {
                errors.Add($"حدث خطأ: {ex.Message}");
                return (false, errors.ToArray());
            }
        }

        public async Task<(bool Success, string[] Errors)> UpdateOrderStage1Async(OrderStage1ViewModel model, IFormFileCollection files, string userId, string userName)
        {
            var errors = new List<string>();

            var order = await _unitOfWork.Orders.GetByIdAsync(model.Id);
            if (order == null)
            {
                errors.Add("الطلب غير موجود");
                return (false, errors.ToArray());
            }

            if (await OrderNumberExistsAsync(model.OrderNumber, model.Id))
            {
                errors.Add("رقم الطلب موجود مسبقاً");
                return (false, errors.ToArray());
            }

            try
            {
                order.OrderNumber = model.OrderNumber;
                order.OrderDate = model.OrderDate;
                order.ExpectedDeliveryDate = model.ExpectedDeliveryDate;
                order.CustomerId = model.CustomerId;
                order.SupplierId = model.SupplierId;
                order.ProductId = model.ProductId;
                order.RollDirectionId = model.RollDirectionId;
                order.RawMaterialId = model.RawMaterialId;
                order.Length = model.Length;
                order.Width = model.Width;
                order.Quantity = model.Quantity;
                order.OrderNotes = model.OrderNotes;
                order.LastModified = DateTime.Now;
                order.ModifiedBy = userId;

                _unitOfWork.Orders.Update(order);

                // رفع ملفات جديدة
                if (files != null && files.Count > 0)
                {
                    await UploadOrderFilesAsync(order.Id, files, userId, errors);
                }

                // إضافة Timeline Entry
                await _unitOfWork.OrderTimelines.AddTimelineEntryAsync(
                    order.Id,
                    order.Stage,
                    order.Status,
                    "تم تحديث بيانات الطلب",
                    "تم تحديث البيانات في مرحلة الطلب",
                    userId,
                    userName
                );

                await _unitOfWork.CompleteAsync();

                return (true, errors.ToArray());
            }
            catch (Exception ex)
            {
                errors.Add($"حدث خطأ: {ex.Message}");
                return (false, errors.ToArray());
            }
        }

        // ===== مرحلة المراجعة (Stage 2) =====

        public async Task<(bool Success, string[] Errors)> MoveToReviewAsync(int orderId, string userId, string userName)
        {
            var errors = new List<string>();

            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
            {
                errors.Add("الطلب غير موجود");
                return (false, errors.ToArray());
            }

            if (order.Stage != OrderStage.Order)
            {
                errors.Add("الطلب ليس في مرحلة الطلب");
                return (false, errors.ToArray());
            }

            try
            {
                order.Stage = OrderStage.Review;
                order.Status = OrderStatus.UnderReview;
                order.LastModified = DateTime.Now;
                order.ModifiedBy = userId;

                _unitOfWork.Orders.Update(order);

                // إضافة Timeline Entry
                await _unitOfWork.OrderTimelines.AddTimelineEntryAsync(
                    order.Id,
                    OrderStage.Review,
                    OrderStatus.UnderReview,
                    "انتقل الطلب إلى مرحلة المراجعة",
                    null,
                    userId,
                    userName
                );

                await _unitOfWork.CompleteAsync();

                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                errors.Add($"حدث خطأ: {ex.Message}");
                return (false, errors.ToArray());
            }
        }

        public async Task<IEnumerable<OrderViewModel>> GetReviewOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetReviewOrdersAsync();
            return orders.Select(MapToViewModel);
        }

        public async Task<OrderStage2ViewModel> GetOrderForReviewAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetWithAllDetailsAsync(id);
            if (order == null)
                return null;

            return new OrderStage2ViewModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                CustomerName = order.Customer?.CustomerName,
                ProductName = order.Product?.ProductName,
                Quantity = order.Quantity,
                Length = order.Length,
                Width = order.Width,
                RawMaterialName = order.RawMaterial?.RawMaterialName,
                MachineId = order.MachineId,
                CoreId = order.CoreId,
                KnifeId = order.KnifeId,
                CartonId = order.CartonId,
                MoldId = order.MoldId,
                ReviewNotes = order.ReviewNotes,
                SelectedManufacturingAdditions = order.ManufacturingItems?
                    .Select(mi => mi.ManufacturingAdditionId)
                    .ToList() ?? new List<int>()
            };
        }

        public async Task<(bool Success, string[] Errors)> UpdateOrderStage2Async(OrderStage2ViewModel model, string userId, string userName)
        {
            var errors = new List<string>();

            var order = await _unitOfWork.Orders.GetByIdAsync(model.Id);
            if (order == null)
            {
                errors.Add("الطلب غير موجود");
                return (false, errors.ToArray());
            }

            try
            {
                order.MachineId = model.MachineId;
                order.CoreId = model.CoreId;
                order.KnifeId = model.KnifeId;
                order.CartonId = model.CartonId;
                order.MoldId = model.MoldId;
                order.ReviewNotes = model.ReviewNotes;
                order.ReviewedDate = DateTime.Now;
                order.ReviewedBy = userId;
                order.LastModified = DateTime.Now;
                order.ModifiedBy = userId;

                _unitOfWork.Orders.Update(order);

                // حذف الإضافات التصنيعية القديمة
                var existingItems = await _unitOfWork.OrderManufacturingItems.FindAsync(mi => mi.OrderId == order.Id);
                foreach (var item in existingItems)
                {
                    _unitOfWork.OrderManufacturingItems.Delete(item);
                }

                // إضافة الإضافات التصنيعية الجديدة
                if (model.SelectedManufacturingAdditions != null && model.SelectedManufacturingAdditions.Any())
                {
                    int displayOrder = 1;
                    foreach (var additionId in model.SelectedManufacturingAdditions)
                    {
                        var manufacturingItem = new OrderManufacturingItem
                        {
                            
                            OrderId = order.Id,
                            ManufacturingAdditionId = additionId,
                            IsCompleted = false,
                            DisplayOrder = displayOrder++
                        };

                        await _unitOfWork.OrderManufacturingItems.AddAsync(manufacturingItem);
                    }
                }

                // إضافة Timeline Entry
                await _unitOfWork.OrderTimelines.AddTimelineEntryAsync(
                    order.Id,
                    OrderStage.Review,
                    OrderStatus.UnderReview,
                    "تم مراجعة الطلب",
                    model.ReviewNotes,
                    userId,
                    userName
                );

                await _unitOfWork.CompleteAsync();

                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                errors.Add($"حدث خطأ: {ex.Message}");
                return (false, errors.ToArray());
            }
        }

        public async Task<(bool Success, string[] Errors)> MoveToManufacturingAsync(int orderId, string userId, string userName)
        {
            var errors = new List<string>();

            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
            {
                errors.Add("الطلب غير موجود");
                return (false, errors.ToArray());
            }

            if (order.Stage != OrderStage.Review)
            {
                errors.Add("الطلب ليس في مرحلة المراجعة");
                return (false, errors.ToArray());
            }

            // بيانات المراجعة اختيارية - يمكن الانتقال للتصنيع بدونها
            
            try
            {
                order.Stage = OrderStage.Manufacturing;
                order.Status = OrderStatus.InManufacturing;
                order.ManufacturingStartDate = DateTime.Now;
                order.LastModified = DateTime.Now;
                order.ModifiedBy = userId;

                _unitOfWork.Orders.Update(order);

                // إضافة Timeline Entry
                await _unitOfWork.OrderTimelines.AddTimelineEntryAsync(
                    order.Id,
                    OrderStage.Manufacturing,
                    OrderStatus.InManufacturing,
                    "انتقل الطلب إلى مرحلة التصنيع",
                    null,
                    userId,
                    userName
                );

                await _unitOfWork.CompleteAsync();

                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                errors.Add($"حدث خطأ: {ex.Message}");
                return (false, errors.ToArray());
            }
        }

        // ===== مرحلة التصنيع (Stage 3) =====

        public async Task<IEnumerable<OrderViewModel>> GetManufacturingOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetManufacturingOrdersAsync();
            return orders.Select(MapToViewModel);
        }

        public async Task<OrderStage3ViewModel> GetOrderForManufacturingAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetWithAllDetailsAsync(id);
            if (order == null)
                return null;

            var manufacturingItems = await _unitOfWork.OrderManufacturingItems.GetByOrderIdAsync(order.Id);

            return new OrderStage3ViewModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                CustomerName = order.Customer?.CustomerName,
                ProductName = order.Product?.ProductName,
                Quantity = order.Quantity,
                ManufacturingItems = manufacturingItems.Select(mi => new ManufacturingItemViewModel
                {
                    Id = mi.Id,
                    ManufacturingAdditionId = mi.ManufacturingAdditionId,
                    AdditionName = mi.ManufacturingAddition?.AdditionName,
                    IsCompleted = mi.IsCompleted,
                    CompletedDate = mi.CompletedDate,
                    CompletedByName = mi.CompletedBy
                }).ToList(),
                ManufacturingNotes = order.ManufacturingNotes
            };
        }

        public async Task<(bool Success, string[] Errors)> CompleteManufacturingItemAsync(int orderId, int itemId, string userId, string userName)
        {
            var errors = new List<string>();

            var item = await _unitOfWork.OrderManufacturingItems.GetByIdAsync(itemId);
            if (item == null || item.OrderId != orderId)
            {
                errors.Add("العنصر غير موجود");
                return (false, errors.ToArray());
            }

            if (item.IsCompleted)
            {
                errors.Add("العنصر مكتمل بالفعل");
                return (false, errors.ToArray());
            }

            try
            {
                item.IsCompleted = true;
                item.CompletedDate = DateTime.Now;
                item.CompletedBy = userId;

                _unitOfWork.OrderManufacturingItems.Update(item);

                // إضافة Timeline Entry
                var addition = await _unitOfWork.ManufacturingAdditions.GetByIdAsync(item.ManufacturingAdditionId);
                await _unitOfWork.OrderTimelines.AddTimelineEntryAsync(
                    orderId,
                    OrderStage.Manufacturing,
                    OrderStatus.InManufacturing,
                    $"تم إكمال: {addition?.AdditionName}",
                    null,
                    userId,
                    userName
                );

                await _unitOfWork.CompleteAsync();

                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                errors.Add($"حدث خطأ: {ex.Message}");
                return (false, errors.ToArray());
            }
        }

        public async Task<bool> CheckAllManufacturingCompletedAsync(int orderId)
        {
            return await _unitOfWork.OrderManufacturingItems.AreAllItemsCompletedAsync(orderId);
        }

        public async Task<(bool Success, string[] Errors)> MoveToPrintingAsync(int orderId, string userId, string userName)
        {
            var errors = new List<string>();

            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
            {
                errors.Add("الطلب غير موجود");
                return (false, errors.ToArray());
            }

            if (order.Stage != OrderStage.Manufacturing)
            {
                errors.Add("الطلب ليس في مرحلة التصنيع");
                return (false, errors.ToArray());
            }

            // التحقق من اكتمال جميع عناصر التصنيع
            var allCompleted = await CheckAllManufacturingCompletedAsync(orderId);
            if (!allCompleted)
            {
                errors.Add("يجب إكمال جميع عناصر التصنيع قبل الانتقال للطباعة");
                return (false, errors.ToArray());
            }

            try
            {
                order.Stage = OrderStage.Printing;
                order.Status = OrderStatus.InPrinting;
                order.ManufacturingEndDate = DateTime.Now;
                order.LastModified = DateTime.Now;
                order.ModifiedBy = userId;

                _unitOfWork.Orders.Update(order);

                // إضافة Timeline Entry
                await _unitOfWork.OrderTimelines.AddTimelineEntryAsync(
                    order.Id,
                    OrderStage.Printing,
                    OrderStatus.InPrinting,
                    "انتقل الطلب إلى مرحلة الطباعة",
                    null,
                    userId,
                    userName
                );

                await _unitOfWork.CompleteAsync();

                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                errors.Add($"حدث خطأ: {ex.Message}");
                return (false, errors.ToArray());
            }
        }

        // ===== مرحلة الطباعة (Stage 4) =====

        public async Task<IEnumerable<OrderViewModel>> GetPrintingOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetPrintingOrdersAsync();
            return orders.Select(MapToViewModel);
        }

        public async Task<OrderStage4ViewModel> GetOrderForPrintingAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetWithAllDetailsAsync(id);
            if (order == null)
                return null;

            return new OrderStage4ViewModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                ExpectedDeliveryDate = order.ExpectedDeliveryDate,
                CustomerName = order.Customer?.CustomerName,
                CustomerPhone = order.Customer?.Phone,
                ProductName = order.Product?.ProductName,
                SupplierName = order.Supplier?.SupplierName,
                Length = order.Length,
                Width = order.Width,
                Quantity = order.Quantity,
                RawMaterialName = order.RawMaterial?.RawMaterialName,
                RollDirectionNumber = order.RollDirection?.DirectionNumber.ToString(),
                RollDirectionImage = order.RollDirection?.DirectionImage,
                MachineName = order.Machine?.MachineName,
                CoreName = order.Core?.CoreName.ToString(),
                KnifeName = order.Knife?.KnifeName.ToString(),
                CartonName = order.Carton?.CartonName,
                MoldName = order.Mold?.MoldNumber.ToString(),
                OrderNotes = order.OrderNotes,
                ReviewNotes = order.ReviewNotes,
                ManufacturingNotes = order.ManufacturingNotes,
                PrintingNotes = order.PrintingNotes,
                PrintingStartDate = order.PrintingStartDate,
                PrintingEndDate = order.PrintingEndDate,
                PrintedBy = order.PrintedBy
            };
        }

        public async Task<(bool Success, string[] Errors)> StartPrintingAsync(int orderId, string userId, string userName)
        {
            var errors = new List<string>();

            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
            {
                errors.Add("الطلب غير موجود");
                return (false, errors.ToArray());
            }

            if (order.Stage != OrderStage.Printing)
            {
                errors.Add("الطلب ليس في مرحلة الطباعة");
                return (false, errors.ToArray());
            }

            if (order.PrintingStartDate.HasValue)
            {
                errors.Add("تم بدء الطباعة مسبقاً");
                return (false, errors.ToArray());
            }

            try
            {
                order.PrintingStartDate = DateTime.Now;
                order.LastModified = DateTime.Now;
                order.ModifiedBy = userId;

                _unitOfWork.Orders.Update(order);

                // إضافة Timeline Entry
                await _unitOfWork.OrderTimelines.AddTimelineEntryAsync(
                    order.Id,
                    OrderStage.Printing,
                    OrderStatus.InPrinting,
                    "بدء الطباعة",
                    null,
                    userId,
                    userName
                );

                await _unitOfWork.CompleteAsync();

                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                errors.Add($"حدث خطأ: {ex.Message}");
                return (false, errors.ToArray());
            }
        }

        public async Task<(bool Success, string[] Errors)> CompletePrintingAsync(int orderId, string userId, string userName)
        {
            var errors = new List<string>();

            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
            {
                errors.Add("الطلب غير موجود");
                return (false, errors.ToArray());
            }

            if (order.Stage != OrderStage.Printing)
            {
                errors.Add("الطلب ليس في مرحلة الطباعة");
                return (false, errors.ToArray());
            }

            if (!order.PrintingStartDate.HasValue)
            {
                errors.Add("يجب بدء الطباعة أولاً");
                return (false, errors.ToArray());
            }

            try
            {
                order.Stage = OrderStage.Completed;
                order.Status = OrderStatus.Completed;
                order.PrintingEndDate = DateTime.Now;
                order.ActualDeliveryDate = DateTime.Now;
                order.PrintedBy = userId;
                order.LastModified = DateTime.Now;
                order.ModifiedBy = userId;

                _unitOfWork.Orders.Update(order);

                // إضافة Timeline Entry
                await _unitOfWork.OrderTimelines.AddTimelineEntryAsync(
                    order.Id,
                    OrderStage.Completed,
                    OrderStatus.Completed,
                    "تم إكمال الطلب بنجاح",
                    null,
                    userId,
                    userName
                );

                await _unitOfWork.CompleteAsync();

                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                errors.Add($"حدث خطأ: {ex.Message}");
                return (false, errors.ToArray());
            }
        }
        // ===== البحث والفلترة =====

        public async Task<IEnumerable<OrderViewModel>> GetOrdersByStatusAsync(OrderStatus status)
        {
            var orders = await _unitOfWork.Orders.GetByStatusAsync(status);
            return orders.Select(MapToViewModel);
        }

        public async Task<IEnumerable<OrderViewModel>> GetOrdersByStageAsync(OrderStage stage)
        {
            var orders = await _unitOfWork.Orders.GetByStageAsync(stage);
            return orders.Select(MapToViewModel);
        }

        public async Task<IEnumerable<OrderViewModel>> GetOrdersByCustomerAsync(int customerId)
        {
            var orders = await _unitOfWork.Orders.GetByCustomerAsync(customerId);
            return orders.Select(MapToViewModel);
        }

        public async Task<IEnumerable<OrderViewModel>> GetOrdersBySupplierAsync(int supplierId)
        {
            var orders = await _unitOfWork.Orders.GetBySupplierAsync(supplierId);
            return orders.Select(MapToViewModel);
        }

        public async Task<IEnumerable<OrderViewModel>> GetOrdersByProductAsync(int productId)
        {
            var orders = await _unitOfWork.Orders.GetByProductAsync(productId);
            return orders.Select(MapToViewModel);
        }

        public async Task<IEnumerable<OrderViewModel>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByDateRangeAsync(startDate, endDate);
            return orders.Select(MapToViewModel);
        }

        public async Task<IEnumerable<OrderViewModel>> SearchOrdersAsync(string searchTerm)
        {
            var orders = await _unitOfWork.Orders.SearchAsync(searchTerm);
            return orders.Select(MapToViewModel);
        }

        // ===== الإحصائيات =====

        public async Task<int> GetTotalOrdersCountAsync()
        {
            return await _unitOfWork.Orders.GetTotalOrdersCountAsync();
        }

        public async Task<int> GetActiveOrdersCountAsync()
        {
            return await _unitOfWork.Orders.GetActiveOrdersCountAsync();
        }

        public async Task<int> GetOrdersCountByStatusAsync(OrderStatus status)
        {
            return await _unitOfWork.Orders.GetOrdersCountByStatusAsync(status);
        }

        public async Task<int> GetOrdersCountByStageAsync(OrderStage stage)
        {
            return await _unitOfWork.Orders.GetOrdersCountByStageAsync(stage);
        }

        public async Task<IEnumerable<OrderViewModel>> GetLateOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetLateOrdersAsync();
            return orders.Select(MapToViewModel);
        }

        public async Task<IEnumerable<OrderViewModel>> GetTodayOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetTodayOrdersAsync();
            return orders.Select(MapToViewModel);
        }

        /*public async Task<OrderStatisticsViewModel> GetOrderStatisticsAsync()
        {
            var totalOrders = await GetTotalOrdersCountAsync();
            var activeOrders = await GetActiveOrdersCountAsync();
            var pendingOrders = await GetOrdersCountByStatusAsync(OrderStatus.Pending);
            var underReviewOrders = await GetOrdersCountByStatusAsync(OrderStatus.UnderReview);
            var inManufacturingOrders = await GetOrdersCountByStatusAsync(OrderStatus.InManufacturing);
            var inPrintingOrders = await GetOrdersCountByStatusAsync(OrderStatus.InPrinting);
            var completedOrders = await GetOrdersCountByStatusAsync(OrderStatus.Completed);
            var cancelledOrders = await GetOrdersCountByStatusAsync(OrderStatus.Cancelled);
            var lateOrders = (await GetLateOrdersAsync()).Count();
            var todayOrders = (await GetTodayOrdersAsync()).Count();

            return new OrderStatisticsViewModel
            {
                TotalOrders = totalOrders,
                ActiveOrders = activeOrders,
                PendingOrders = pendingOrders,
                UnderReviewOrders = underReviewOrders,
                InManufacturingOrders = inManufacturingOrders,
                InPrintingOrders = inPrintingOrders,
                CompletedOrders = completedOrders,
                CancelledOrders = cancelledOrders,
                LateOrders = lateOrders,
                TodayOrders = todayOrders
            };
        }*/
        public async Task<OrderStatisticsViewModel> GetOrderStatisticsAsync()
        {
            var now = DateTime.Now;
            var today = now.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var yearStart = new DateTime(now.Year, 1, 1);

            var allOrders = await _unitOfWork.Orders.GetAllAsync();

            var statistics = new OrderStatisticsViewModel
            {
                // إحصائيات عامة
                TotalOrders = allOrders.Count(),
                ActiveOrders = allOrders.Count(o => o.IsActive),
                InactiveOrders = allOrders.Count(o => !o.IsActive),

                // حسب الحالة
                PendingOrders = allOrders.Count(o => o.Status == OrderStatus.Pending),
                UnderReviewOrders = allOrders.Count(o => o.Status == OrderStatus.UnderReview),
                InProgressOrders = allOrders.Count(o =>
                    o.Status == OrderStatus.InManufacturing ||
                    o.Status == OrderStatus.InPrinting),
                InManufacturingOrders = allOrders.Count(o => o.Status == OrderStatus.InManufacturing),
                InPrintingOrders = allOrders.Count(o => o.Status == OrderStatus.InPrinting),
                CompletedOrders = allOrders.Count(o => o.Status == OrderStatus.Completed),
                CancelledOrders = allOrders.Count(o => o.Status == OrderStatus.Cancelled),
                OnHoldOrders = allOrders.Count(o => o.Status == OrderStatus.OnHold),

                // حسب المرحلة
                OrderStageCount = allOrders.Count(o => o.Stage == OrderStage.Order),
                ReviewStageCount = allOrders.Count(o => o.Stage == OrderStage.Review),
                ManufacturingStageCount = allOrders.Count(o => o.Stage == OrderStage.Manufacturing),
                PrintingStageCount = allOrders.Count(o => o.Stage == OrderStage.Printing),
                CompletedStageCount = allOrders.Count(o => o.Stage == OrderStage.Completed),

                // التأخير
                LateOrders = allOrders.Count(o =>
                    o.ExpectedDeliveryDate < now &&
                    o.Status != OrderStatus.Completed &&
                    o.Status != OrderStatus.Cancelled),

                // اليوم
                TodayOrders = allOrders.Count(o => o.OrderDate.Date == today),
                TodayCompletedOrders = allOrders.Count(o =>
                    o.ActualDeliveryDate.HasValue &&
                    o.ActualDeliveryDate.Value.Date == today),
                TodayCancelledOrders = allOrders.Count(o =>
                    o.Status == OrderStatus.Cancelled &&
                    o.LastModified.HasValue &&
                    o.LastModified.Value.Date == today),

                // الأسبوع
                WeekOrders = allOrders.Count(o => o.OrderDate >= weekStart),
                WeekCompletedOrders = allOrders.Count(o =>
                    o.ActualDeliveryDate.HasValue &&
                    o.ActualDeliveryDate.Value >= weekStart),

                // الشهر
                MonthOrders = allOrders.Count(o => o.OrderDate >= monthStart),
                MonthCompletedOrders = allOrders.Count(o =>
                    o.ActualDeliveryDate.HasValue &&
                    o.ActualDeliveryDate.Value >= monthStart),

                // السنة
                YearOrders = allOrders.Count(o => o.OrderDate >= yearStart),
                YearCompletedOrders = allOrders.Count(o =>
                    o.ActualDeliveryDate.HasValue &&
                    o.ActualDeliveryDate.Value >= yearStart),

                // الكميات
                TotalQuantity = allOrders.Sum(o => o.Quantity),
                CompletedQuantity = allOrders
                    .Where(o => o.Status == OrderStatus.Completed)
                    .Sum(o => o.Quantity),
                PendingQuantity = allOrders
                    .Where(o => o.Status != OrderStatus.Completed && o.Status != OrderStatus.Cancelled)
                    .Sum(o => o.Quantity)
            };

            // حساب النسب
            if (statistics.TotalOrders > 0)
            {
                statistics.LateOrdersPercentage = Math.Round(
                    (decimal)statistics.LateOrders / statistics.TotalOrders * 100, 2);

                statistics.CompletionRate = Math.Round(
                    (decimal)statistics.CompletedOrders / statistics.TotalOrders * 100, 2);
            }

            // متوسط الطلبات اليومية
            var firstOrderDate = allOrders.Min(o => o.OrderDate);
            var daysSinceFirstOrder = (now - firstOrderDate).Days + 1;
            statistics.AverageDailyOrders = daysSinceFirstOrder > 0
                ? Math.Round((decimal)statistics.TotalOrders / daysSinceFirstOrder, 2)
                : 0;

            // متوسط وقت الإنجاز
            var completedOrders = allOrders
                .Where(o => o.Status == OrderStatus.Completed && o.ActualDeliveryDate.HasValue)
                .ToList();

            if (completedOrders.Any())
            {
                var totalDays = completedOrders
                    .Sum(o => (o.ActualDeliveryDate!.Value - o.OrderDate).TotalDays);

                statistics.AverageCompletionTime = Math.Round(
                    (decimal)totalDays / completedOrders.Count, 2);
            }

            return statistics;
        }

        // ===== تغيير الحالة والمرحلة =====

        public async Task<(bool Success, string[] Errors)> ChangeOrderStatusAsync(int id, OrderStatus status, string userId, string userName)
        {
            var errors = new List<string>();

            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
            {
                errors.Add("الطلب غير موجود");
                return (false, errors.ToArray());
            }

            try
            {
                var oldStatus = order.Status;
                order.Status = status;
                order.LastModified = DateTime.Now;
                order.ModifiedBy = userId;

                _unitOfWork.Orders.Update(order);

                // إضافة Timeline Entry
                await _unitOfWork.OrderTimelines.AddTimelineEntryAsync(
                    id,
                    order.Stage,
                    status,
                    $"تغيير الحالة من {oldStatus.GetDisplayName()} إلى {status.GetDisplayName()}",
                    null,
                    userId,
                    userName
                );

                await _unitOfWork.CompleteAsync();

                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                errors.Add($"حدث خطأ: {ex.Message}");
                return (false, errors.ToArray());
            }
        }

        public async Task<(bool Success, string[] Errors)> ChangeOrderStageAsync(int id, OrderStage stage, string userId, string userName)
        {
            var errors = new List<string>();

            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
            {
                errors.Add("الطلب غير موجود");
                return (false, errors.ToArray());
            }

            try
            {
                var oldStage = order.Stage;
                order.Stage = stage;
                order.LastModified = DateTime.Now;
                order.ModifiedBy = userId;

                _unitOfWork.Orders.Update(order);

                // إضافة Timeline Entry
                await _unitOfWork.OrderTimelines.AddTimelineEntryAsync(
                    id,
                    stage,
                    order.Status,
                    $"تغيير المرحلة من {oldStage.GetDisplayName()} إلى {stage.GetDisplayName()}",
                    null,
                    userId,
                    userName
                );

                await _unitOfWork.CompleteAsync();

                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                errors.Add($"حدث خطأ: {ex.Message}");
                return (false, errors.ToArray());
            }
        }

        public async Task<(bool Success, string[] Errors)> DeleteOrderAsync(int id)
        {
            var errors = new List<string>();

            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
            {
                errors.Add("الطلب غير موجود");
                return (false, errors.ToArray());
            }

            try
            {
                // Soft Delete
                order.IsActive = false;
                order.LastModified = DateTime.Now;

                _unitOfWork.Orders.Update(order);
                await _unitOfWork.CompleteAsync();

                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                errors.Add($"حدث خطأ: {ex.Message}");
                return (false, errors.ToArray());
            }
        }

        public async Task<(bool Success, string[] Errors)> CancelOrderAsync(int id, string reason, string userId, string userName)
        {
            var errors = new List<string>();

            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
            {
                errors.Add("الطلب غير موجود");
                return (false, errors.ToArray());
            }

            if (order.Status == OrderStatus.Completed)
            {
                errors.Add("لا يمكن إلغاء طلب مكتمل");
                return (false, errors.ToArray());
            }

            if (order.Status == OrderStatus.Cancelled)
            {
                errors.Add("الطلب ملغي بالفعل");
                return (false, errors.ToArray());
            }

            try
            {
                order.Status = OrderStatus.Cancelled;
                order.LastModified = DateTime.Now;
                order.ModifiedBy = userId;

                _unitOfWork.Orders.Update(order);

                // إضافة Timeline Entry
                await _unitOfWork.OrderTimelines.AddTimelineEntryAsync(
                    order.Id,
                    order.Stage,
                    OrderStatus.Cancelled,
                    "تم إلغاء الطلب",
                    reason,
                    userId,
                    userName
                );

                await _unitOfWork.CompleteAsync();

                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                errors.Add($"حدث خطأ: {ex.Message}");
                return (false, errors.ToArray());
            }
        }

        // ===== Timeline =====

        public async Task<IEnumerable<OrderTimelineViewModel>> GetOrderTimelineAsync(int orderId)
        {
            var timeline = await _unitOfWork.OrderTimelines.GetByOrderIdAsync(orderId);
            return timeline.OrderByDescending(t => t.ActionDate).Select(MapTimelineToViewModel);
        }

        // ===== Helper Methods =====

        private async Task UploadOrderFilesAsync(int orderId, IFormFileCollection files, string userId, List<string> errors)
        {
            var folderPath = $"uploads/orders/{orderId}";

            var uploadResult = await _fileUploadService.UploadFilesAsync(files, folderPath, _allowedExtensions, _maxFileSize);

            if (uploadResult.Errors.Any())
            {
                errors.AddRange(uploadResult.Errors);
            }

            // حفظ معلومات الملفات في قاعدة البيانات
            foreach (var filePath in uploadResult.FilePaths)
            {
                var fileIndex = uploadResult.FilePaths.IndexOf(filePath);
                if (fileIndex < files.Count)
                {
                    var file = files[fileIndex];

                    var attachment = new OrderAttachment
                    {
                        OrderId = orderId,
                        FileName = Path.GetFileName(filePath),
                        OriginalFileName = file.FileName,
                        FilePath = filePath,
                        FileSize = file.Length,
                        FileExtension = Path.GetExtension(file.FileName).ToLowerInvariant(),
                        ContentType = _fileUploadService.GetContentType(file.FileName),
                        UploadedDate = DateTime.Now,
                        UploadedBy = userId,
                        IsActive = true
                    };

                    await _unitOfWork.OrderAttachments.AddAsync(attachment);
                }
            }

            await _unitOfWork.CompleteAsync();
        }

        private OrderViewModel MapToViewModel(Order order)
        {
            return new OrderViewModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                ExpectedDeliveryDate = order.ExpectedDeliveryDate,
                ActualDeliveryDate = order.ActualDeliveryDate,
                CustomerName = order.Customer?.CustomerName,
                SupplierName = order.Supplier?.SupplierName,
                ProductName = order.Product?.ProductName,
                RollDirectionNumber = order.RollDirection?.DirectionNumber.ToString(),
                RollDirectionImage = order.RollDirection?.DirectionImage,
                RawMaterialName = order.RawMaterial?.RawMaterialName,
                Quantity = order.Quantity,
                Length = order.Length,
                Width = order.Width,
                Status = order.Status,
                Stage = order.Stage,
                CreatedDate = order.CreatedDate,
                LastModified = order.LastModified,
                IsActive = order.IsActive,
                IsLate = order.ExpectedDeliveryDate < DateTime.Today && order.Stage != OrderStage.Completed,
                Priority = order.Priority,
                OrderNotes = order.OrderNotes,
                AttachmentsCount = order.Attachments?.Count ?? 0,
                ManufacturingItemsCount = order.ManufacturingItems?.Count ?? 0
            };
        }

        private OrderAttachmentViewModel MapAttachmentToViewModel(OrderAttachment attachment)
        {
            return new OrderAttachmentViewModel
            {
                Id = attachment.Id,
                FileName = attachment.FileName,
                OriginalFileName = attachment.OriginalFileName,
                FilePath = attachment.FilePath,
                FileSize = attachment.FileSize,
                FileExtension = attachment.FileExtension,
                UploadedDate = attachment.UploadedDate
            };
        }

        private ManufacturingItemViewModel MapManufacturingItemToViewModel(OrderManufacturingItem item)
        {
            return new ManufacturingItemViewModel
            {
                Id = item.Id,
                ManufacturingAdditionId = item.ManufacturingAdditionId,
                AdditionName = item.ManufacturingAddition?.AdditionName,
                IsCompleted = item.IsCompleted,
                CompletedDate = item.CompletedDate,
                CompletedByName = item.CompletedBy
            };
        }

        private OrderTimelineViewModel MapTimelineToViewModel(OrderTimeline timeline)
        {
            return new OrderTimelineViewModel
            {
                Id = timeline.Id,
                Stage = timeline.Stage,
                Status = timeline.Status,
                Action = timeline.Action,
                Notes = timeline.Notes,
                ActionDate = timeline.ActionDate,
                ActionByName = timeline.ActionByName
            };
        }

        // ===== Print Queue & Priority Management =====

        public async Task<IEnumerable<PrintQueueViewModel>> GetPrintQueueOrderedByPriorityAsync()
        {
            var includes = new List<string>() { nameof(Customer) , nameof(Product) };
            // Get only orders in Printing stage (Stage 4)
            var orders = (await _unitOfWork.Orders.GetWithIncludesAsync(includes))
                .Where(o => 
                o.IsActive && 
                o.Stage == OrderStage.Printing);

            return orders
                .OrderBy(o => o.Priority)
                .ThenBy(o => o.ExpectedDeliveryDate)
                .Select(MapToPrintQueueViewModel);
        }

        public async Task<PrintQueueViewModel> GetPrintQueueItemAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            return order != null ? MapToPrintQueueViewModel(order) : null;
        }

        public async Task<(bool Success, string[] Errors)> UpdatePrintOrderPriorityAsync(int orderId, int priority, string userId, string userName)
        {
            var errors = new List<string>();

            if (priority < 1)
            {
                errors.Add("الأولوية يجب أن تكون رقم موجب");
                return (false, errors.ToArray());
            }

            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
            {
                errors.Add("الطلب غير موجود");
                return (false, errors.ToArray());
            }

            var oldPriority = order.Priority;
            order.Priority = priority;
            order.LastModified = DateTime.Now;
            order.ModifiedBy = userId;

            _unitOfWork.Orders.Update(order);

            var timeline = new OrderTimeline
            {
                OrderId = orderId,
                Stage = order.Stage,
                Status = order.Status,
                Action = $"تم تغيير الأولوية من {oldPriority} إلى {priority}",
                ActionDate = DateTime.Now,
                ActionBy = userId,
                ActionByName = userName
            };

            await _unitOfWork.OrderTimelines.AddAsync(timeline);

            try
            {
                await _unitOfWork.CompleteAsync();
                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                errors.Add($"حدث خطأ أثناء تحديث الأولوية: {ex.Message}");
                return (false, errors.ToArray());
            }
        }

        public async Task<(bool Success, string[] Errors)> ReorderPrintQueueAsync(Dictionary<int, int> orderPriorities, string userId, string userName)
        {
            var errors = new List<string>();

            try
            {
                foreach (var kvp in orderPriorities)
                {
                    var order = await _unitOfWork.Orders.GetByIdAsync(kvp.Key);
                    if (order != null)
                    {
                        order.Priority = kvp.Value;
                        order.LastModified = DateTime.Now;
                        order.ModifiedBy = userId;
                        _unitOfWork.Orders.Update(order);
                    }
                }

                var timeline = new OrderTimeline
                {
                    OrderId = orderPriorities.Keys.First(),
                    Stage = OrderStage.Printing,
                    Status = OrderStatus.InPrinting,
                    Action = "تم إعادة ترتيب قائمة انتظار الطباعة",
                    ActionDate = DateTime.Now,
                    ActionBy = userId,
                    ActionByName = userName
                };

                await _unitOfWork.OrderTimelines.AddAsync(timeline);
                await _unitOfWork.CompleteAsync();

                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                errors.Add($"حدث خطأ أثناء إعادة الترتيب: {ex.Message}");
                return (false, errors.ToArray());
            }
        }

        private PrintQueueViewModel MapToPrintQueueViewModel(Order order)
        {
            return new PrintQueueViewModel
            {
                Id = order.Id,
                Priority = order.Priority,
                OrderNumber = order.OrderNumber,
                
                // Stage 1 - Order Info
                CustomerName = order.Customer?.CustomerName,
                SupplierName = order.Supplier?.SupplierName,
                ProductName = order.Product?.ProductName,
                RollDirectionNumber = order.RollDirection?.DirectionNumber.ToString(),
                RollDirectionImage = order.RollDirection?.DirectionImage,
                RawMaterialName = order.RawMaterial?.RawMaterialName,
                Length = order.Length,
                Width = order.Width,
                Quantity = order.Quantity,
                
                // Stage 2 - Review Info
                MachineName = order.Machine?.MachineName,
                CoreName = order.Core?.CoreName,
                KnifeName = order.Knife?.KnifeName,
                CartonName = order.Carton?.CartonName,
                MoldNumber = order.Mold?.MoldNumber,
                ReviewedBy = order.ReviewedBy,
                
                // Stage 4 - Printing Info
                PrintingStartDate = order.PrintingStartDate,
                PrintedBy = order.PrintedBy,
                
                // Dates
                OrderDate = order.OrderDate,
                ExpectedDeliveryDate = order.ExpectedDeliveryDate,
                CreatedDate = order.CreatedDate,
                LastModified = order.LastModified,
                
                // Status
                Status = order.Status,
                Stage = order.Stage,
                IsActive = order.IsActive,
                IsLate = order.ExpectedDeliveryDate < DateTime.Today && order.Stage != OrderStage.Completed,
                
                // Audit
                CreatedBy = order.CreatedBy,
                ModifiedBy = order.ModifiedBy,
                
                // Counts
                AttachmentsCount = order.Attachments?.Count ?? 0,
                ManufacturingItemsCount = order.ManufacturingItems?.Count ?? 0
            };
        }
    }
}