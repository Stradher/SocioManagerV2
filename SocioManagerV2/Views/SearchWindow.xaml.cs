using System.Windows;
using System.Windows.Input;
using SocioManagerV2.ViewModels;

namespace SocioManagerV2.Views
{
    public partial class SearchWindow : Window
    {
        public SearchWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is SearchViewModel viewModel && viewModel.SelectCommand.CanExecute(null))
            {
                viewModel.SelectCommand.Execute(null);
                DialogResult = true;
                Close();
            }
        }
    }
}
