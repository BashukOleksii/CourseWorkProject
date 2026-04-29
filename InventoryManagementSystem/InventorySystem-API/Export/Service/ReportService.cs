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
using System.ComponentModel.Design;

namespace InventorySystem_API.Report.Service
{
    public class ReportService : IReportService
    {
        private readonly IInventoryService _inventoryService;
        private readonly IWarehouseService _warehouseService;
        private readonly IUserService _userService;

        public ReportService(IInventoryService inventoryService, IWarehouseService warehouseService, IUserService userService)
        {
            _inventoryService = inventoryService;
            _warehouseService = warehouseService;
            _userService = userService;
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

        private void AddHeader(PageDescriptor page, string middle)
        {
            page.Header().Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("Звіт про товари").FontSize(20).SemiBold();
                    column.Item().Text(middle).FontSize(12);
                    column.Item().Text($"Створено: {DateTime.Now}").FontSize(12);
                });
            });
        }

        private void AddFooter(PageDescriptor page)
        {
            page.Footer().AlignCenter().Text(x =>
            {
                x.Span("Сторінка ");
                x.CurrentPageNumber();
                x.Span(" з ");
                x.TotalPages().FontSize(10);
            });
        }

        public async Task<byte[]> GetInventoryReport(InventoryQuery? inventoryQuery, string warehouseId)
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

                        AddHeader(page, $"Id складу: {warehouseId}");

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
                                    columns.ConstantColumn(50);
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

                        AddFooter(page);

                    });
                }
            });

            return documnent.GeneratePdf();
        }

        public async Task<byte[]> GetWarehouseReport(WarehouseQuery? warehouseQuery, string companyId)
        {
            var warehouseData = await _warehouseService.Get(companyId,warehouseQuery);

            if (warehouseData.Count == 0)
                throw new InvalidOperationException("Немає даних для генерації звіту.");

            var totalCount = warehouseData.Count;
            var totalArea = warehouseData.Sum(w => w.Area);
            int totalEmployees = 0;

            long[] countEmployee = new long[warehouseData.Count];
            for(int i = 0; i < warehouseData.Count; i++)
                countEmployee[i] = await _userService.GetCountInWarehouse(warehouseData[i].Id);
            

            var documnent = Document.Create(container =>
            {
                {
                    container.Page(page =>
                    {
                        page.Margin(50);
                        page.Size(PageSizes.A4);
                        page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Verdana));

                        AddHeader(page, $"Id компанії: {companyId}");

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
                                table.Cell().Element(Block).Text($"Загальна площі: {totalArea:N2}");
                                table.Cell().Element(Block).Text($"Загальна кількість працівників: {countEmployee.Length}");
                            });

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(50);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellHeaderStyle).Text("Id");
                                    header.Cell().Element(CellHeaderStyle).Text("Назва");
                                    header.Cell().Element(CellHeaderStyle).Text("Місто");
                                    header.Cell().Element(CellHeaderStyle).Text("Вулиця");
                                    header.Cell().Element(CellHeaderStyle).Text("Будинок");
                                    header.Cell().Element(CellHeaderStyle).Text("Площа");
                                    header.Cell().Element(CellHeaderStyle).Text("К-ть працівників");
                                });

                                for(int i = 0; i< warehouseData.Count; i++)
                                {
                                    table.Cell().Element(CellContentStyle).Text(warehouseData[i].Id);
                                    table.Cell().Element(CellContentStyle).Text(warehouseData[i].Name);
                                    table.Cell().Element(CellContentStyle).Text(warehouseData[i].Address.City);
                                    table.Cell().Element(CellContentStyle).Text(warehouseData[i].Address.Street);
                                    table.Cell().Element(CellContentStyle).Text(warehouseData[i].Address.HouseNumber);
                                    table.Cell().Element(CellContentStyle).Text(warehouseData[i].Area.ToString());
                                    table.Cell().Element(CellContentStyle).Text(countEmployee[i].ToString());
                                }
                            });



                        });

                        AddFooter(page);

                    });
                }
            });

            return documnent.GeneratePdf();
        }

        public async Task<byte[]> GetUserReport(UserQuery? userQuery, string companyId)
        {
            var usersData = await _userService.Get(companyId, userQuery,null);

            if (usersData.Count == 0)
                throw new InvalidOperationException("Немає даних для генерації звіту.");

            var totalCount = usersData.Count;

            var allWarehouseIds = usersData
                .Where(user => user.UserRole != UserRole.admin && user.WarehouseIds is not null)
                .SelectMany(user => user.WarehouseIds)
                .Distinct()
                .ToArray();

            var warehouseDict = (await _warehouseService.GetByIds(allWarehouseIds,companyId))
                .ToDictionary(warehouse => warehouse.Id, warehouse => warehouse.Name);

            var documnent = Document.Create(container =>
            {
                {
                    container.Page(page =>
                    {
                        page.Margin(50);
                        page.Size(PageSizes.A4);
                        page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Verdana));

                        AddHeader(page, $"Id компанії: {companyId}");

                        page.Content().PaddingVertical(10).Column(column =>
                        {
                            column.Item().PaddingBottom(10).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                });

                                table.Cell().Element(Block).Text($"Загальна кількість: {totalCount}");
                            });

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(50);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1.5f);
                                    columns.RelativeColumn(3);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellHeaderStyle).Text("Id");
                                    header.Cell().Element(CellHeaderStyle).Text("Ім'я");
                                    header.Cell().Element(CellHeaderStyle).Text("Роль");
                                    header.Cell().Element(CellHeaderStyle).Text("Список складів");
                                });

                                for (int i = 0; i < usersData.Count; i++)
                                {
                                    var user = usersData[i];
                                    table.Cell().Element(CellContentStyle).Text(user.Id);
                                    table.Cell().Element(CellContentStyle).Text(user.Name);
                                    table.Cell().Element(CellContentStyle).Text(user.UserRole.ToString());

                                    table.Cell().Element(CellContentStyle).PaddingVertical(2).Column(listStack =>
                                    {
                                        if (user.UserRole == UserRole.admin)
                                            listStack.Item().Text("• Всі доступні");
                                        else if(user.WarehouseIds is null)
                                            listStack.Item().Text("• Немає переліку");
                                        else
                                        {
                                            foreach (var warehouseId in user.WarehouseIds)
                                            {
                                                var warehouseName = warehouseDict.ContainsKey(warehouseId) ? warehouseDict[warehouseId] : "Невідомий склад";
                                                listStack.Item().Text($"• {warehouseName}");
                                            }
                                        }

                                    });
                                }
                            });



                        });

                        AddFooter(page);

                    });
                }
            });

            return documnent.GeneratePdf();
        }
    }
}
