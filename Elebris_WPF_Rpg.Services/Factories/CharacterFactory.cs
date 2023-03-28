using Elebris_WPF_Rpg.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Elebris_WPF_Rpg.Services.Factories
{
    public static class CharacterFactory
    {
        public static Player ReturnPlayer(string name, ObservableCollection<ValueDataModel> attributes)
        {
            return new Player(name, 0, 10, 10, attributes, 10);
        }


    }
}
