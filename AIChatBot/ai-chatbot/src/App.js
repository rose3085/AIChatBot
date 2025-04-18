import React, { useState } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import './App.css';

function App() {
  const [input, setInput] = useState('');
  const [messages , setMessages] = useState([]);

  const sendMessage = async () => {
    if (!input.trim()) return;
  
    const userMessage = { sender: 'user', text: input };
  
    // Add user message first
    setMessages((prev) => [...prev, userMessage]);
    setInput('');
  
    try {
      debugger;
      const response = await fetch('https://localhost:7078/postQuestions', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ question: input })  // send user's message to API
      });
  
      const data = await response.text();
      const botMessage = {
        sender: 'bot',
        text: data || 'Sorry, I could not understand that.'
      };
  
      setMessages((prev) => [...prev, botMessage]);






    } catch (error) {
      console.error('Error sending message:', error);
      setMessages((prev) => [
        ...prev,
        { sender: 'bot', text: 'Oops! Something went wrong.' }
      ]);
    }
  };
  





  return (
    <div className="container py-5">
      <div className="row justify-content-center">
      <div className="col-md-15 col-lg-10">
      <div className="card shadow-lg">
        <div className="card-header bg-primary text-white">
          <h5 className="mb-0">AI Chatbot</h5>
        </div>
        <div className="card-body chat-box" style={{ maxHeight: '70vh', overflowY: 'auto' }}>
          {messages.map((msg, idx) => (
            <div
              key={idx}
              className={`d-flex mb-2 ${
                msg.sender === 'user' ? 'justify-content-end' : 'justify-content-start'
              }`}
            >
              <div
                className={`p-2 rounded ${
                  msg.sender === 'user' ? 'bg-primary text-white' : 'bg-light'
                }`}
                style={{ maxWidth: '70%',  whiteSpace: 'pre-line' }}
              >
                {msg.text}
              </div>
            </div>
          ))}
        </div>
        <div className="card-footer d-flex">
          <input
            type="text"
            className="form-control me-2"
            placeholder="Type your message..."
            value={input}
            onChange={(e) => setInput(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && sendMessage()}
          />
          <button className="btn btn-primary" onClick={sendMessage}>
            Send
          </button>
        </div>
      </div>
      </div>
      </div>
    </div>
  );
}

export default App;



