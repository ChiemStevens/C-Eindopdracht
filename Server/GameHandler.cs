using Newtonsoft.Json;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

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

        public GameHandler(int maxRounds, Room gameRoom)
        {
            this.maxRounds = maxRounds;
            this.gameRoom = gameRoom;

            this.currentRoundNumber = 1;
            this.started = false;
            this.word = "";
            this.usersAndPoints = new Dictionary<string, int>();
            this.usersGuessedCorrect = new List<string>();
        }

        public void StartGame(List<ClientThread> clients)
        {
            if(!this.started)
            {
                this.started = true;
                foreach(ClientThread client in clients)
                {
                    usersAndPoints.Add(client.Name, 0);
                }
                ReadWordsAndChoice();
            }
        }

        public void EndGame()
        {
            if(this.started)
            {
                this.started = false;
                this.gameRoom.SendToAllClientsInRoom(new Message(MessageTypes.EndGame, JsonConvert.SerializeObject(new EndGameModel(usersAndPoints))));
            }
        }

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
                        Console.WriteLine("New round");
                        //New round
                        currentRoundNumber++;
                        usersGuessedCorrect.Clear();
                        ReadWordsAndChoice();
                        this.gameRoom.NextDrawer();

                        List<Message> messages = new List<Message>();
                        messages.Add(new Message(MessageTypes.GuessWord, JsonConvert.SerializeObject(new GuessModel(this.Word))));
                        messages.Add(new Message(MessageTypes.NewRound, JsonConvert.SerializeObject(new GameModel(this.Word.Length, currentRoundNumber))));
                        this.gameRoom.SendToAllClientsInRoom(messages);
                    }
                }

                return true;
            }

            this.gameRoom.SendToAllClientsInRoom(new Message(MessageTypes.Inform, JsonConvert.SerializeObject(new GuessModel(clientName + ": " + word))));
            return false;
        }

        private void ReadWordsAndChoice()
        {
            string[] lines = System.IO.File.ReadAllLines(@"words.txt");
            word = lines[new Random().Next(0, lines.Length - 1)];
            Console.WriteLine(word);
        }

        public string Word { get { return this.word; } }
    }
}
