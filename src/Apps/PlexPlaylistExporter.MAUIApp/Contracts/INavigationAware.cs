using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexPlaylistExporter.MAUIApp.Contracts
{
    public interface INavigationAware
    {
        Task OnNavigatedTo();
        Task OnNavigatedFrom();
    }
}
