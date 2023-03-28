using Elebris_WPF_Rpg.Models;
using Elebris_WPF_Rpg.Services.Factories;

namespace Elebris_Unit_Tests
{
    public class Tests
    {
        List<ValueDataModel> _attributes;
        [SetUp]
        public void Setup()
        {
            _attributes = new List<ValueDataModel>();
        }

        [Test]
        public void Player_Creates_Equals_True()
        {
            CharacterFactory.ReturnPlayer("Jake", _attributes);
        }
    }
}