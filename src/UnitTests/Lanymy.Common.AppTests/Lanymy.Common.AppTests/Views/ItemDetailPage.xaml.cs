using Lanymy.Common.AppTests.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace Lanymy.Common.AppTests.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}