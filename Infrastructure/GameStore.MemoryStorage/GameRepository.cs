using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace GameStore.MemoryStorage
{
    public class GameRepository : IGameRepository
    {
        private readonly Game[] games;
        private GameImageData gamesImageData;
        public GameRepository(IOptions<GameImageData> settings)
        {
            gamesImageData = settings.Value;
            InitialiseGamesData(ref games);

        }

        private void InitialiseGamesData(ref Game[] games)
        {
            games = new[]
            {
                new Game(1, "Grand Theft Auto V [GTA 5]", "Rockstar",
                        "Grand Theft Auto V на Playstation 4 – продолжение криминального боевика с сатирой на современную действительность. В этот раз история возвращается в город Лос-Сантос, " +
                        "прообразом которого послужил Лос-Анджелес. Главных героя сразу три: Майкл, Франклин и Тревор. У каждого свои умения и особенности. ",
                        "Шутеры/Action",2590m, gamesImageData.GTA.ParseHexString(), "2013" ),

                new Game(2,"FIFA 21","Electronic Arts",
                        "Что готовит нам сезон грядущий? EA SPORTS – дружная команда поклонников спорта и видеоигр, которые из раза в раз стараются перенести бесценный опыт соревновательного духа на виртуальное пространство," +
                        " делая его доступным практически каждому! И вот, теперь и вы можете купить FIFA 21 на базе движка Frostbite, став героем множества турниров, будь то UEFA Champions League или CONMEBOL Libertadores! Сплотите команду мечты и хозяйничайте на игровых полях улиц и стадионов!Вы – сердце этого зрелищного спорта!",
                        "Симуляторы", 2150m, gamesImageData.FIFA21.ParseHexString(), "2020"),

                new Game(3, "Fallout 76", "Bethesda Softworks",
                        "Западная Вирджиния вновь становится обителью разумной жизни! Апокалипсис превратил некогда прекрасный штат в пустошь, но это не повод унывать для его обитателей! Решившись купить Fallout 76 Wastelanders и встретить негодяев и мирных жителей, вы откроете для себя совершенно новую главу в истории серии игр Fallout! Впрочем, вам нет нужды встречать все испытания и невзгоды в одиночку, ибо вы можете позвать с собой друзей, демонстрируя завидную взаимовыручку, когда дело касается отстрела мутировавшей фауны и людей, чьи моральные нормы тоже прошли своего рода мутацию!",
                        "Ролевые игры",735m, gamesImageData.Fallout.ParseHexString(), "2018"),

                new Game(4, "Wasteland 3","inXile Entertainment",
                        "Радиоактивные пустоши взывают к вам!Высоко оцененная серия ролевых игр от inXile Entertainment продолжает своё победоносное шествие, выпустив третью часть занимательной вселенной постапокалипсиса. Решившись купить Wasteland 3 и повлиять на судьбы многих, на судьбу всей Аризоны. Вам придётся посетить раскаленные пустыни, холодные горы, потерять всё и даже построить новую базу. Собственными руками вы построите нечто, что станет маяком безопасности и надёжности в опасном мире, полный опасных фракций, интриг, сумасшедших культистов, банд головорезов и хищников!",
                        "Стратегии",1599m, gamesImageData.Wasteland.ParseHexString(), "28 августа 2020")
             
            };
        }
     
        public Game[] GetAllByCategory(string category)
        {
            
            return games.Where(game => game.Category.Contains(category)).ToArray();
        }

        public Game[] GetAllByNameOrPublisher(string nameOrPublisher)
        {
            return nameOrPublisher != null ? games.Where(game => game.Name.ToLower().Contains((nameOrPublisher).ToLower())
                                       || game.Publisher.ToLower().Contains((nameOrPublisher).ToLower())).ToArray() : new Game[0];
        }

        public Game GetGameById(int id)
        {
            return games.Single(game => game.Id == id);
        }

        public Game[] GetGamesByIds(IEnumerable<int> gamesId)
        {
            return games.Join(gamesId, game => game.Id, id => id, (game,id) => game).ToArray();
        }
    

    }

}
