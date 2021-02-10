using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStore.MemoryStorage
{
    public class GameMemoryRepository : IGameMemoryRepository
    {
        private readonly GameMemoryStorage[] games;
        private GameImageData gamesImageData;
        public GameMemoryRepository(IOptions<GameImageData> settings)
        {
            gamesImageData = settings.Value;
            InitialiseGamesData(ref games);

        }

        private void InitialiseGamesData(ref GameMemoryStorage[] games)
        {
            games = new[]
            {
                new GameMemoryStorage(1, "Grand Theft Auto V [GTA 5]", "Rockstar",
                        "Grand Theft Auto V на Playstation 4 – продолжение криминального боевика с сатирой на современную действительность.",
                        "Grand Theft Auto V на Playstation 4 – продолжение криминального боевика с сатирой на современную действительность. В этот раз история возвращается в город Лос-Сантос, " +
                        "прообразом которого послужил Лос-Анджелес. Главных героя сразу три: Майкл, Франклин и Тревор. У каждого свои умения и особенности. ",
                        "Шутеры/Action", 2590m, gamesImageData.GTA.ParseHexString(), "2013", new DateTime(2019, 6, 23, 18, 30, 25) ),

                new GameMemoryStorage(2,"FIFA 21","Electronic Arts",
                        "Серия компьютерных игр в жанре симулятора футбола, которая разрабатывается студией EA Canada, входящей в состав корпорации Electronic Arts. ",
                        "Что готовит нам сезон грядущий? EA SPORTS – дружная команда поклонников спорта и видеоигр, которые из раза в раз стараются перенести бесценный опыт соревновательного духа на виртуальное пространство," +
                        " делая его доступным практически каждому! И вот, теперь и вы можете купить FIFA 21 на базе движка Frostbite, став героем множества турниров, будь то UEFA Champions League или CONMEBOL Libertadores! Сплотите команду мечты и хозяйничайте на игровых полях улиц и стадионов!Вы – сердце этого зрелищного спорта!",
                        "Симуляторы", 2150m, gamesImageData.FIFA21.ParseHexString(), "2020", new DateTime(2017, 7, 5, 12, 15, 17)),

                new GameMemoryStorage(3, "Fallout 76", "Bethesda Softworks",
                        "Действие происходит после ядерной войны на территории США, которая превратилась в радиоактивную пустыню и охвачена анархией.",
                        "Западная Вирджиния вновь становится обителью разумной жизни! Апокалипсис превратил некогда прекрасный штат в пустошь, но это не повод унывать для его обитателей! Решившись купить Fallout 76 Wastelanders и встретить негодяев и мирных жителей, вы откроете для себя совершенно новую главу в истории серии игр Fallout! Впрочем, вам нет нужды встречать все испытания и невзгоды в одиночку, ибо вы можете позвать с собой друзей, демонстрируя завидную взаимовыручку, когда дело касается отстрела мутировавшей фауны и людей, чьи моральные нормы тоже прошли своего рода мутацию!",
                        "Ролевые игры",735m, gamesImageData.Fallout.ParseHexString(), "2018", new DateTime(2018, 2, 6, 5, 17, 16)),

                new GameMemoryStorage(4, "Wasteland 3","inXile Entertainment",
                        "Радиоактивные пустоши взывают к вам! Это продолжение серии ролевых игр, положивших начало жанру постапокалипсиса в видеоиграх",
                        "Радиоактивные пустоши взывают к вам! Высоко оцененная серия ролевых игр от inXile Entertainment продолжает своё победоносное шествие, выпустив третью часть занимательной вселенной постапокалипсиса. Решившись купить Wasteland 3 и повлиять на судьбы многих, на судьбу всей Аризоны. Вам придётся посетить раскаленные пустыни, холодные горы, потерять всё и даже построить новую базу. Собственными руками вы построите нечто, что станет маяком безопасности и надёжности в опасном мире, полный опасных фракций, интриг, сумасшедших культистов, банд головорезов и хищников!",
                        "Стратегии",1599m, gamesImageData.Wasteland.ParseHexString(), "28 августа 2020", new DateTime(2020, 1, 7, 12, 13, 17))
             
            };
        }
     
        public GameMemoryStorage[] GetAllByCategory(string category)
        {
            
            return games.Where(game => game.Category.Contains(category)).ToArray();
        }

        public GameMemoryStorage[] GetAllByNameOrPublisher(string nameOrPublisher)
        {
            return nameOrPublisher != null ? games.Where(game => game.Name.ToLower().Contains((nameOrPublisher).ToLower())
                                       || game.Publisher.ToLower().Contains((nameOrPublisher).ToLower())).ToArray() : new GameMemoryStorage[0];
        }

        public GameMemoryStorage GetGameById(int id)
        {
            return games.Single(game => game.Id == id);
        }

        public GameMemoryStorage[] GetGamesByIds(IEnumerable<int> gamesId)
        {
            return games.Join(gamesId, game => game.Id, id => id, (game,id) => game).ToArray();
        }

        public GameMemoryStorage[] GetLastSixGameByDataAdding()
        {
            return games.OrderByDescending(game => game.DateOfAdding).Take(6).ToArray();
        }


    }

}
