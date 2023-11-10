using EticaretProje.Models;

namespace EticaretProje.ViewModels
{
    //class içerisindeki her alan viewda kullanılmayacaksa, view genelinde ihtiyaç duydugumuz alanlar,classlar bir başka class içerisinde birleştirilir
    public class ShoppingCartViewModel
    {
        public List<Sepet> CartItems { get; set; }
        public decimal CartTotal { get; set; }
    }
}
