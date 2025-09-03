namespace BlazorEquipManage.Models
{
    public class Category
    {
        public required int Code { get; set; }
        public int? Parent {  get; set; }
        public required string Name { get; set; }
    }
}
