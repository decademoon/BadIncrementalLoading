using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

namespace BadIncrementalLoading
{
    class IncrementalLoadingCollection : ObservableCollection<string>, ISupportIncrementalLoading
    {
        int offset = 0;

        public bool HasMoreItems { get { return offset < 1000; } }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            var dispatcher = Window.Current.Dispatcher;

            return AsyncInfo.Run(async cancelToken =>
            {
                System.Diagnostics.Debug.WriteLine("Loading {0} items", count);

                //await Task.Run(() => { });

                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    for (var i = offset; i < offset + count; i++)
                        Add(i.ToString());
                });
                    
                offset += (int)count;

                return new LoadMoreItemsResult { Count = count };
            });
        }
    }

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            listview.ItemsSource = new IncrementalLoadingCollection();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void listview_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            stats.Text = listview.Items.Count.ToString();
        }
    }
}
