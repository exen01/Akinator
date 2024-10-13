using System.IO;
using System.Text.Json;

namespace Akinator
{
    /// <summary>
    /// Игра акинатор.
    /// </summary>
    class AkinatorGame
    {
        #region Поля и свойства

        /// <summary>
        /// Корневой узел в дереве.
        /// </summary>
        public Node RootNode { get; private set; }

        /// <summary>
        /// Текущий узел в дереве.
        /// </summary>
        private Node _currentNode;

        /// <summary>
        /// Путь к файлу с базой знаний.
        /// </summary>
        private const string FilePath = "knowledge_base.json";

        /// <summary>
        /// Событие обновления вопроса.
        /// </summary>
        public event Action<string> OnQuestionUpdated;

        /// <summary>
        /// Событие конца игры.
        /// </summary>
        public event Action<string> OnGameOver;

        /// <summary>
        /// Событие отображения базы знаний.
        /// </summary>
        public event Action<string> OnKnowledgeBaseShow;

        #endregion

        #region Методы

        /// <summary>
        /// Запуск новой игры.
        /// </summary>
        public void StartNewGame()
        {
            AskQuestion(_currentNode);
        }

        /// <summary>
        /// Задает вопрос из узла игроку.
        /// </summary>
        /// <param name="node">Узел с вопросом</param>
        private void AskQuestion(Node node)
        {
            OnQuestionUpdated?.Invoke(node.IsQuestion ? node.Data : $"Это {node.Data} ?");
        }

        /// <summary>
        /// Обрабатывает ответ игрока.
        /// </summary>
        /// <param name="isYes">Ответ игрока</param>
        public void HandleAnswer(bool isYes)
        {
            if (_currentNode.IsQuestion)
            {
                _currentNode = isYes ? _currentNode.YesBranch : _currentNode.NoBranch;
                AskQuestion(_currentNode);
            }
            else
            {
                OnGameOver?.Invoke(isYes ? "Ура, я угадал!" : "Сдаюсь. Какой подарок вы загадали?");
            }
        }

        /// <summary>
        /// Добавляет информацию о новом подарке в базу знаний.
        /// </summary>
        /// <param name="newItem">Новая информация</param>
        /// <param name="question">Новый вопрос</param>
        /// <param name="isYesAnswer">Ответ на новый вопрос</param>
        public void LearnNewGift(string newItem, string question, bool isYesAnswer)
        {
            Node oldNode = new Node(_currentNode);
            Node newNode = new Node { Data = newItem };

            _currentNode.Data = question;
            if (isYesAnswer)
            {
                _currentNode.YesBranch = newNode;
                _currentNode.NoBranch = oldNode;
            }
            else
            {
                _currentNode.YesBranch = oldNode;
                _currentNode.NoBranch = newNode;
            }

            SaveKnowledgeBase();
        }

        /// <summary>
        /// Загружает базу знаний из файла.
        /// </summary>
        private void LoadKnowledgeBase()
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                RootNode = JsonSerializer.Deserialize<Node>(json);
            }
            else
            {
                RootNode = new Node { Data = "Это съедобное?" };
                RootNode.YesBranch = new Node { Data = "Шоколад" };
                RootNode.NoBranch = new Node { Data = "Цветы" };
            }
        }

        /// <summary>
        /// Сохраняет базу знаний в файл.
        /// </summary>
        private void SaveKnowledgeBase()
        {
            string json = JsonSerializer.Serialize(RootNode, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        /// <summary>
        /// Отображает базу знаний.
        /// </summary>
        public void ShowKnowledgeBase()
        {
            RootNode.PrintPretty("", false, OnKnowledgeBaseShow);
        }

        #endregion

        #region Конструкторы

        /// <summary>
        /// Конструктор.
        /// </summary>
        public AkinatorGame()
        {
            LoadKnowledgeBase();
            _currentNode = RootNode;
        }

        #endregion
    }
}