using dCom.ViewModel;
using System.Windows;

namespace dCom
{
	/// <summary>
	/// Interaction logic for ControlWindow.xaml
	/// </summary>
	internal partial class ControlWindow : Window
	{
		public ControlWindow()
		{
			InitializeComponent();
		}

		public ControlWindow(BasePointItem dataContext) : this()
		{
			this.DataContext = dataContext;
			Title = string.Format("Control Window - {0}", dataContext.Name);
		}

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}