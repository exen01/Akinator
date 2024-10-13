using System.Diagnostics;

namespace Akinator
{
    /// <summary>
    /// Представляет узел в дереве.
    /// </summary>
    public class Node
    {
        #region Поля и свойства

        /// <summary>
        /// Данные, которые хранятся в узле.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Ветка "Да".
        /// </summary>
        public Node YesBranch { get; set; }

        /// <summary>
        /// Ветка "Нет".
        /// </summary>
        public Node NoBranch { get; set; }

        /// <summary>
        /// Дочерние элементы узла.
        /// </summary>
        public List<Node> Children
        {
            get
            {
                if (YesBranch == null || NoBranch == null)
                {
                    if (YesBranch == null && NoBranch != null)
                    {
                        return new List<Node> { NoBranch };
                    }

                    if (NoBranch == null && YesBranch != null)
                    {
                        return new List<Node> { YesBranch };
                    }

                    return new List<Node>();
                }

                return new List<Node> { YesBranch, NoBranch };
            }
        }

        /// <summary>
        /// Является ли узел вопросом.
        /// </summary>
        public bool IsQuestion => YesBranch != null && NoBranch != null;

        #endregion

        #region Методы

        /// <summary>
        /// Текстовое представление дерева.
        /// </summary>
        /// <param name="indent">Разделитель</param>
        /// <param name="last">Является ли узел последним</param>
        /// <param name="print">Метод печати дерева</param>
        public void PrintPretty(string indent, bool last, Action<string> print)
        {
            print(indent);
            if (last)
            {
                print("\\-");
                indent += "  ";
            }
            else
            {
                print("|-");
                indent += "| ";
            }

            print(Data + '\n');

            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].PrintPretty(indent, i == Children.Count - 1, print);
            }
        }

        #endregion

        #region Конструкторы

        /// <summary>
        /// Конструктор.
        /// </summary>
        public Node()
        {
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="data">Данные, которые хранятся в узле</param>
        public Node(Node data)
        {
            Data = data.Data;
            YesBranch = data.YesBranch;
            NoBranch = data.NoBranch;
        }

        #endregion
    }
}