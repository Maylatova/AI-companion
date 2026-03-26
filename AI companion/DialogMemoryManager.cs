using System.Collections.Generic;
using System.Linq;

namespace AI_companion
{
    public class ChatMessage
    {
        public string Role { get; set; } 
        public string Content { get; set; }
    }

    public class DialogMemoryManager
    {
        private readonly List<ChatMessage> _history;
        private readonly int _maxHistoryLength;

        public DialogMemoryManager(int maxHistoryLength = 10, string systemPrompt = "Ти корисний ШІ-асистент. Виконуй команди коротко та чітко.")
        {
            _history = new List<ChatMessage>();
            _maxHistoryLength = maxHistoryLength;

            AddMessage("system", systemPrompt);
        }

        public void AddMessage(string role, string content)
        {
            _history.Add(new ChatMessage { Role = role, Content = content });

            if (_history.Count > _maxHistoryLength + 1)
            {
                _history.RemoveAt(1);
            }
        }

        public List<ChatMessage> GetHistory()
        {
            return _history.ToList();
        }

        public void ClearHistory()
        {
            var systemPrompt = _history[0];
            _history.Clear();
            _history.Add(systemPrompt);
        }
    }
}
