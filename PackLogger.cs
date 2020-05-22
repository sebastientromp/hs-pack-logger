

namespace PackLogger
{
    using HackF5.UnitySpy.HearthstoneLib;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    class PackLogger
    {
        // DLL libraries used to manage hotkeys
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private MindVision _mindVision;

        private MindVision MindVision
        {
            get
            {
                if (_mindVision == null)
                {
                    try
                    {
                        _mindVision = new MindVision();
                        Console.WriteLine("MinVision created", "");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Could not instantiate MindVision: " + e.Message, e.StackTrace);
                    }
                }
                return _mindVision;
            }

        }

        private bool _watching = true;
        private IList<IPackCard> previousPack = new List<IPackCard>();

        public async Task<bool> Start()
        {
            while (_watching)
            {
                //Console.WriteLine("Looking for new pack");
                await Task.Delay(500);
                var openPacksInfo = this.MindVision.GetOpenPacksInfo();
                if (openPacksInfo == null)
                {
                    continue;
                }
                var cards = openPacksInfo.PackOpening.Cards;
                if (cards?.Count == 5)
                {

                    if (cards.All(card => previousPack.Any(previous =>
                        previous.CardId == card.CardId && previous.Premium == card.Premium && previous.IsNew == card.IsNew)))
                    {
                        continue;
                    }

                    previousPack = cards;
                    var packInfo = new PackInfo
                    {
                        booster_type = openPacksInfo.LastOpenedBoosterId,
                        date = DateTime.Now,
                        cards = cards
                            .Select(card => new CardInfo
                            {
                                cardId = card.CardId,
                                premium = card.Premium,
                            })
                            .ToList(),
                        accountId = MindVision.GetAccountInfo(),
                    };
                    string serializedPackInfo = JsonConvert.SerializeObject(packInfo);
                    Console.WriteLine(serializedPackInfo);
                    var systemPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                    var fullPath = Path.Combine(systemPath, "logWatcher.txt");
                    using (StreamWriter sw = File.AppendText(fullPath))
                    {
                        sw.WriteLine(serializedPackInfo);
                    }
                }
            }
            return true;
        }
    }

    class PackInfo
    {
        public int booster_type { get; set; }

        public DateTime date { get; set; }

        public IList<CardInfo> cards { get; set; }

        public IAccountInfo accountId { get; set; }
    }

    class CardInfo
    {
        public string cardId { get; set; }

        public bool premium { get; set; }
    }
}
