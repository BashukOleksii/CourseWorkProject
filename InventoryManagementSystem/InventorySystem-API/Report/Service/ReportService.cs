using InventorySystem_API.Inventory.Service;
using InventorySystem_API.Loging.Service;
using InventorySystem_API.User.Services;
using InventorySystem_API.Warehouse.Service;
using InventorySystem_Shared.Inventory;
using InventorySystem_Shared.Loging;
using InventorySystem_Shared.User;
using InventorySystem_Shared.Warehouse;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InventorySystem_API.Report.Service
{
    public class ReportService : IReportService
    {
        private readonly IInventoryService _inventoryService;
        private readonly IWarehouseService _warehouseService;
        private readonly IUserService _userService;
        private readonly ILogService _logService;

        public ReportService(IInventoryService inventoryService, IWarehouseService warehouseService, IUserService userService, ILogService logService)
        {
            _inventoryService = inventoryService;
            _warehouseService = warehouseService;
            _userService = userService;
            _logService = logService;
        }

        private IContainer Block(IContainer container) =>
            container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten1)
                .Background(Colors.Grey.Lighten4)
                .Padding(5)
                .AlignCenter()
                .AlignMiddle();

        private IContainer CellHeaderStyle(IContainer container) =>
            container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);

        private IContainer CellContentStyle(IContainer container) =>
            container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);


        public async Task<byte[]> GetInventoryReport(InventoryQuery inventoryQuery, string warehouseId)
        {
            var inventoryData = await _inventoryService.Get(inventoryQuery, warehouseId);

            if(inventoryData.Count == 0)
                throw new InvalidOperationException("Немає даних для генерації звіту.");

            var totalCount = inventoryData.Sum(item => item.Quantity);
            var totalSum = inventoryData.Sum(item => item.Price * item.Quantity);
            var maxPrice = inventoryData.Max(item => item.Price);
            var minPrice = inventoryData.Min(item => item.Price);
            var averagePrice = inventoryData.Average(item => item.Price);

            var documnent = Document.Create(container =>
            {
                {
                    container.Page(page =>
                    {
                        page.Margin(50);
                        page.Size(PageSizes.A4);
                        page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Verdana));

                        page.Header().Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("Звіт про товари").FontSize(20).SemiBold();
                                column.Item().Text($"Id складу: {warehouseId}").FontSize(12);
                                column.Item().Text($"Створено: {DateTime.Now}").FontSize(12);
                            });
                        });

                        page.Content().PaddingVertical(10).Column(column =>
                        {
                            column.Item().PaddingBottom(10).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                table.Cell().Element(Block).Text($"Загальна кількість: {totalCount}");
                                table.Cell().Element(Block).Text($"Загальна вартість: {totalSum:N2}");
                                table.Cell().Element(Block).Text($"Середня вартість: {averagePrice:N2}");
                            });

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(30);
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellHeaderStyle).Text("Id");
                                    header.Cell().Element(CellHeaderStyle).Text("Назва");
                                    header.Cell().Element(CellHeaderStyle).Text("Тип");
                                    header.Cell().Element(CellHeaderStyle).Text("Виробник");
                                    header.Cell().Element(CellHeaderStyle).Text("Ціна");
                                    header.Cell().Element(CellHeaderStyle).Text("Кількість");
                                });

                                foreach (var item in inventoryData)
                                {
                                    table.Cell().Element(CellContentStyle).Text(item.Id);
                                    table.Cell().Element(CellContentStyle).Text(item.Name);
                                    table.Cell().Element(CellContentStyle).Text(item.InventoryType.ToString());
                                    table.Cell().Element(CellContentStyle).Text(item.Manufacturer?.Name ?? "N/A");
                                    table.Cell().Element(CellContentStyle).Text($"{item.Price:N2}");
                                    table.Cell().Element(CellContentStyle).Text($"{item.Quantity}");
                                }
                            });



                        });

                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.Span("Сторінка ");
                            x.CurrentPageNumber();
                            x.TotalPages().FontSize(10);
                        });

                    });
                }
            });

            return documnent.GeneratePdf();
        }

        public Task<byte[]> GetUserReport(UserQuery userQuery, string companyId)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GetWarehouseReport(WarehouseQuery warehouseQuery, string companyId)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GetWarehouseReport(AuditLogQuery auditLogQuery, string companyId)
        {
            throw new NotImplementedException();
        }
    }
}
