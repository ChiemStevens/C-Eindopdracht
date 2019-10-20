using Microsoft.Win32;
using Newtonsoft.Json;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class GameHandler
    {
        private bool started;
        private string word;
        private Dictionary<string, int> usersAndPoints;
        private List<string> usersGuessedCorrect;
        private int currentRoundNumber;

        private int maxRounds;
        private Room gameRoom;
        private Random rnd;

        public GameHandler(int maxRounds, Room gameRoom)
        {
            this.maxRounds = maxRounds;
            this.gameRoom = gameRoom;

            this.currentRoundNumber = 1;
            this.started = false;
            this.word = "";
            this.usersAndPoints = new Dictionary<string, int>();
            this.usersGuessedCorrect = new List<string>();
            rnd = new Random();
        }

        /// <summary>
        /// Starts the game, if it is not already started, otherwise nothing happens.
        /// </summary>
        /// <param name="clients"></param>
        /// <returns></returns>
        public async Task StartGame(List<ClientThread> clients)
        {
            if(!this.started)
            {
                this.started = true;
                usersAndPoints.Clear();
                this.currentRoundNumber = 1;
                this.usersGuessedCorrect = new List<string>();


                foreach(ClientThread client in clients)
                {
                    usersAndPoints.Add(client.Name, 0);
                }
                this.word = await ReadWordsAndChoiceAsync();
            }
        }

        /// <summary>
        /// Ends the game, only possible when the game is already started
        /// </summary>
        public void EndGame()
        {
            if(this.started)
            {
                this.started = false;
                this.gameRoom.SendToAllClientsInRoom(new Message(MessageTypes.EndGame, JsonConvert.SerializeObject(new EndGameModel(usersAndPoints))));
            }
        }

        /// <summary>
        /// An attempt to guess the current word. If it is not guessed, it is seen as a chat message and send to all other clients.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public bool GuessWord(string word, string clientName)
        {
            if(this.word.ToLower() == word.ToLower())
            {
                this.gameRoom.SendToAllClientsInRoom(new Message(MessageTypes.Inform, JsonConvert.SerializeObject(new GuessModel(clientName + " guessed the word!"))));
                
                Console.WriteLine("Word Guessed by: " + clientName);
                usersAndPoints[clientName] += usersAndPoints.Count - usersGuessedCorrect.Count;
                usersGuessedCorrect.Add(clientName);

                Console.WriteLine("User guessed correct: " + usersGuessedCorrect.Count);
                Console.WriteLine("User and points: " + usersAndPoints.Count);
                if(usersGuessedCorrect.Count == usersAndPoints.Count - 1)
                {
                    if(currentRoundNumber == maxRounds)
                    {
                        Console.WriteLine("Game ending");
                        //End game
                        EndGame();
                    }
                    else
                    {
                        NewRound();
                    }
                }

                return true;
            }

            this.gameRoom.SendToAllClientsInRoom(new Message(MessageTypes.Inform, JsonConvert.SerializeObject(new GuessModel(clientName + ": " + word))));
            return false;
        }

        /// <summary>
        /// Starts a new round.
        /// </summary>
        public async void NewRound()
        {
            Console.WriteLine("New round");
            //New round
            currentRoundNumber++;
            usersGuessedCorrect.Clear();
            this.word = await ReadWordsAndChoiceAsync();
            this.gameRoom.NextDrawer();

            List<Message> messages = new List<Message>();
            messages.Add(new Message(MessageTypes.GuessWord, JsonConvert.SerializeObject(new GuessModel(this.Word))));
            messages.Add(new Message(MessageTypes.NewRound, JsonConvert.SerializeObject(new GameModel(this.Word.Length, currentRoundNumber))));
            this.gameRoom.SendToAllClientsInRoom(messages);
        }

        /*private void ReadWordsAndChoice()
        {
            string[] lines = System.IO.File.ReadAllLines(@"words.txt");
            word = lines[new Random().Next(0, lines.Length - 1)];
            Console.WriteLine(word);
        }*/

        /// <summary>
        /// Read all lines from the file and choices a word that will be drawn.
        /// </summary>
        /// <returns></returns>
        private async Task<string> ReadWordsAndChoiceAsync()
        {
            string path = Directory.GetCurrentDirectory();

            string sourceFile = path + @"\words.txt";

            List<string> lines = new List<string>();
            using (StreamReader reader = File.OpenText(sourceFile))
            {
                string line = null;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lines.Add(line);
                }
            }

            return lines[rnd.Next(0, lines.Count)];
        }

        public string Word { get { return this.word; } }
    }
}
