import React, { useState, useRef } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import './App.css';

function App() {
  const [input, setInput] = useState('');
  const [messages , setMessages] = useState([]);
    const [isRecordingMode, setIsRecordingMode] = useState(false);
    const [isRecording, setIsRecording] = useState(false);
    const [audioURL, setAudioURL] = useState(null);
    const mediaRecorderRef = useRef(null);
    const audioChunksRef = useRef([]);
    const [audio, setAudio] = useState(null);


    const startRecording = async () => {
        const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
        const mediaRecorder = new MediaRecorder(stream);
        mediaRecorderRef.current = mediaRecorder;
        audioChunksRef.current = [];

        mediaRecorder.ondataavailable = (e) => {
            audioChunksRef.current.push(e.data);
        };

        mediaRecorder.onstop = () => {
            const audioBlob = new Blob(audioChunksRef.current, { type: 'audio/webm' });
            setAudio(audioBlob);
            const url = URL.createObjectURL(audioBlob);
            setAudioURL(url);
        };

        mediaRecorder.start();
        setIsRecording(true);
    };

    const stopRecording = () => {
        mediaRecorderRef.current.stop();
        setIsRecording(false);
    };

    const cancelRecording = () => {
        setIsRecordingMode(false);
        setAudioURL(null);
        setIsRecording(false);
    };

    const confirmRecording = async () => {
        debugger;
        if (!audio) {
            console.warn("No audio blob to send.");
            return;
        }

        try {
            const formData = new FormData();
            formData.append("audio", audio, "recording.wav"); // name, file, filename

            const response = await fetch("https://localhost:7078/postAudio", {
                method: "POST",
                body: formData,
            });

            if (!response.ok) {
                throw new Error("Failed to upload audio");
            }

            const result = await response.text();
            console.log("Audio uploaded successfully:", result);

            // You can also append a bot message if response has useful data
            setMessages((prev) => [
                ...prev,
                { sender: "user", text: "[Voice message sent]" },
                { sender: "bot", text: result || "Received your audio!" }
            ]);

        } catch (error) {
            console.error("Error uploading audio:", error);
            setMessages((prev) => [
                ...prev,
                { sender: "bot", text: "Oops! Something went wrong while uploading your audio." }
            ]);
        }

        setIsRecordingMode(false); 
    };


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
        <div className="card-footer d-flex flex-column">
            {!isRecordingMode ? (
                <div className="d-flex w-100">
                    <input
                        type="text"
                        className="form-control me-2"
                        placeholder="Type your message..."
                        value={input}
                        onChange={(e) => setInput(e.target.value)}
                        onKeyDown={(e) => e.key === 'Enter' && sendMessage()}
                    />
                    <button className="btn btn-primary me-2" onClick={sendMessage}>Send</button>
                    <button className="btn btn-secondary" onClick={() => setIsRecordingMode(true)}>Record</button>
                </div>
            ) : (
                <div className="d-flex flex-column align-items-start w-100">
                    {!audioURL ? (
                        <>
                            {!isRecording ? (
                                <button className="btn btn-light mb-2" onClick={startRecording}>
                                    <i className="bi bi-mic-fill fs-4 text-dark"></i>
                                </button>
                            ) : (
                                <button className="btn btn-light mb-2 recording-indicator" onClick={stopRecording}>
                                    <i className="bi bi-record-circle-fill fs-3 text-danger animate-blink"></i>
                                </button>
                            )}
                            <button className="btn btn-secondary" onClick={cancelRecording}>Cancel</button>
                        </>
                    ) : (
                        <>
                            <audio src={audioURL} controls className="mb-2" />
                            <div className="d-flex gap-2">
                                <button className="btn btn-success" onClick={confirmRecording}>Confirm</button>
                                <button className="btn btn-secondary" onClick={cancelRecording}>Cancel</button>
                            </div>
                        </>
                    )}
                </div>
            )}
        </div>
      </div>
      </div>
      </div>
    </div>
  );
}

export default App;



