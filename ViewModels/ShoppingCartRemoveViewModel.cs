namespace EticaretProje.ViewModels
{
    //sepetten birşey silmek istersek kullanılacak model
    public class ShoppingCartRemoveViewModel
    {
        public string Message { get; set; }
        public decimal SepetTotal { get; set; }
        public int SepetCount { get; set; }
        public int ItemCount { get; set; }
        public int DeleteId { get; set; }
    }
}
