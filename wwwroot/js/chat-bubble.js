// Chat Bubble với tích hợp Vapi và Gemini
class ChatBubble {
    constructor() {
        this.isOpen = false;
        this.currentMode = 'chat'; // 'chat' hoặc 'voice'
        this.conversationHistory = [];
        this.isTyping = false;
        this.vapiClient = null;
        this.isCallActive = false;
        this.callDuration = 0;
        this.callTimer = null;
        this.isMuted = false;
        
        this.init();
    }

    init() {
        this.createChatBubble();
        this.attachEventListeners();
        this.initVapi();
    }

    createChatBubble() {
        const container = document.createElement('div');
        container.className = 'chat-bubble-container';
        container.innerHTML = `
            <!-- Nút mở chat bubble -->
            <button class="chat-bubble-button" id="chatBubbleBtn">
                <div class="pulse-ring"></div>
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                    <path d="M12 2C6.48 2 2 6.48 2 12c0 1.54.36 3 .97 4.29L2 22l5.71-.97C9 21.64 10.46 22 12 22c5.52 0 10-4.48 10-10S17.52 2 12 2zm0 18c-1.38 0-2.68-.29-3.86-.81l-.29-.15-3.18.54.54-3.18-.15-.29C4.29 14.68 4 13.38 4 12c0-4.41 3.59-8 8-8s8 3.59 8 8-3.59 8-8 8z"/>
                </svg>
            </button>

            <!-- Cửa sổ chat -->
            <div class="chat-window" id="chatWindow">
                <!-- Header -->
                <div class="chat-header">
                    <div class="chat-header-left">
                        <div class="chat-header-avatar">
                            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                                <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z"/>
                                <circle cx="12" cy="12" r="3"/>
                            </svg>
                        </div>
                        <div class="chat-header-info">
                            <h3>Trợ lý cửa hàng</h3>
                            <p>Luôn sẵn sàng hỗ trợ bạn</p>
                        </div>
                    </div>
                    <div class="chat-header-actions">
                        <button class="chat-header-btn" id="minimizeBtn">
                            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                                <path d="M19 13H5v-2h14v2z"/>
                            </svg>
                        </button>
                    </div>
                </div>

                <!-- Mode Toggle -->
                <div class="chat-mode-toggle">
                    <button class="mode-btn active" data-mode="chat">
                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                            <path d="M20 2H4c-1.1 0-2 .9-2 2v18l4-4h14c1.1 0 2-.9 2-2V4c0-1.1-.9-2-2-2z"/>
                        </svg>
                        Chat
                    </button>
                    <button class="mode-btn" data-mode="voice">
                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                            <path d="M12 14c1.66 0 3-1.34 3-3V5c0-1.66-1.34-3-3-3S9 3.34 9 5v6c0 1.66 1.34 3 3 3z"/>
                            <path d="M17 11c0 2.76-2.24 5-5 5s-5-2.24-5-5H5c0 3.53 2.61 6.43 6 6.92V21h2v-3.08c3.39-.49 6-3.39 6-6.92h-2z"/>
                        </svg>
                        Gọi thoại
                    </button>
                </div>

                <!-- Chat Content -->
                <div class="chat-content" id="chatContent">
                    <div class="chat-messages" id="chatMessages">
                        <div class="message">
                            <div class="message-avatar">
                                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                                    <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z"/>
                                    <circle cx="12" cy="12" r="3"/>
                                </svg>
                            </div>
                            <div class="message-bubble">
                                <p class="message-text">Xin chào! Tôi là trợ lý ảo của cửa hàng. Tôi có thể giúp bạn tìm kiếm sản phẩm, kiểm tra đơn hàng, quản lý khách hàng và nhiều hơn nữa. Bạn cần gì?</p>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Voice Call Container -->
                <div class="voice-call-container" id="voiceCallContainer">
                    <div class="voice-avatar" id="voiceAvatar">
                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                            <path d="M12 14c1.66 0 3-1.34 3-3V5c0-1.66-1.34-3-3-3S9 3.34 9 5v6c0 1.66 1.34 3 3 3z"/>
                            <path d="M17 11c0 2.76-2.24 5-5 5s-5-2.24-5-5H5c0 3.53 2.61 6.43 6 6.92V21h2v-3.08c3.39-.49 6-3.39 6-6.92h-2z"/>
                        </svg>
                    </div>
                    <div class="voice-status" id="voiceStatus">Nhấn để bắt đầu cuộc gọi</div>
                    <div class="voice-duration" id="voiceDuration">00:00</div>
                    <div class="voice-controls" id="voiceControls" style="display: none;">
                        <button class="voice-btn mute" id="muteBtn">
                            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                                <path d="M12 14c1.66 0 3-1.34 3-3V5c0-1.66-1.34-3-3-3S9 3.34 9 5v6c0 1.66 1.34 3 3 3z"/>
                                <path d="M17 11c0 2.76-2.24 5-5 5s-5-2.24-5-5H5c0 3.53 2.61 6.43 6 6.92V21h2v-3.08c3.39-.49 6-3.39 6-6.92h-2z"/>
                            </svg>
                        </button>
                        <button class="voice-btn end-call" id="endCallBtn">
                            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                                <path d="M12 9c-1.6 0-3.15.25-4.6.72v3.1c0 .39-.23.74-.56.9-.98.49-1.87 1.12-2.66 1.85-.18.18-.43.28-.7.28-.28 0-.53-.11-.71-.29L.29 13.08c-.18-.17-.29-.42-.29-.7 0-.28.11-.53.29-.71C3.34 8.78 7.46 7 12 7s8.66 1.78 11.71 4.67c.18.18.29.43.29.71 0 .28-.11.53-.29.71l-2.48 2.48c-.18.18-.43.29-.71.29-.27 0-.52-.11-.7-.28-.79-.74-1.68-1.36-2.66-1.85-.33-.16-.56-.5-.56-.9v-3.1C15.15 9.25 13.6 9 12 9z"/>
                            </svg>
                        </button>
                    </div>
                </div>

                <!-- Chat Input -->
                <div class="chat-input-area" id="chatInputArea">
                    <div class="chat-input-container">
                        <input type="text" class="chat-input" id="chatInput" placeholder="Nhập tin nhắn...">
                        <button class="send-btn" id="sendBtn">
                            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                                <path d="M2.01 21L23 12 2.01 3 2 10l15 2-15 2z"/>
                            </svg>
                        </button>
                    </div>
                </div>
            </div>
        `;

        document.body.appendChild(container);
    }

    attachEventListeners() {
        // Toggle chat window
        document.getElementById('chatBubbleBtn').addEventListener('click', () => {
            this.toggleChat();
        });

        document.getElementById('minimizeBtn').addEventListener('click', () => {
            this.toggleChat();
        });

        // Mode toggle
        document.querySelectorAll('.mode-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                this.switchMode(e.target.closest('.mode-btn').dataset.mode);
            });
        });

        // Send message
        document.getElementById('sendBtn').addEventListener('click', () => {
            this.sendMessage();
        });

        document.getElementById('chatInput').addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                this.sendMessage();
            }
        });

        // Voice controls
        document.getElementById('voiceAvatar').addEventListener('click', () => {
            if (!this.isCallActive) {
                this.startVoiceCall();
            }
        });

        document.getElementById('endCallBtn').addEventListener('click', () => {
            this.endVoiceCall();
        });

        document.getElementById('muteBtn').addEventListener('click', () => {
            this.toggleMute();
        });
    }

    toggleChat() {
        this.isOpen = !this.isOpen;
        const chatWindow = document.getElementById('chatWindow');
        const chatBubbleBtn = document.getElementById('chatBubbleBtn');
        
        if (this.isOpen) {
            chatWindow.classList.add('show');
            chatBubbleBtn.classList.add('active');
        } else {
            chatWindow.classList.remove('show');
            chatBubbleBtn.classList.remove('active');
            if (this.isCallActive) {
                this.endVoiceCall();
            }
        }
    }

    switchMode(mode) {
        this.currentMode = mode;
        
        // Update active button
        document.querySelectorAll('.mode-btn').forEach(btn => {
            btn.classList.remove('active');
        });
        document.querySelector(`[data-mode="${mode}"]`).classList.add('active');

        // Toggle content
        const chatContent = document.getElementById('chatContent');
        const chatInputArea = document.getElementById('chatInputArea');
        const voiceCallContainer = document.getElementById('voiceCallContainer');

        if (mode === 'chat') {
            chatContent.style.display = 'block';
            chatInputArea.style.display = 'block';
            voiceCallContainer.classList.remove('active');
            if (this.isCallActive) {
                this.endVoiceCall();
            }
        } else {
            chatContent.style.display = 'none';
            chatInputArea.style.display = 'none';
            voiceCallContainer.classList.add('active');
        }
    }

    async sendMessage() {
        const input = document.getElementById('chatInput');
        const message = input.value.trim();

        if (!message) return;

        // Add user message to chat
        this.addMessage(message, 'user');
        input.value = '';

        // Show typing indicator
        this.showTypingIndicator();

        try {
            // Send to backend
            const response = await fetch('/api/Assistant/chat', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    message: message,
                    conversationHistory: this.conversationHistory
                })
            });

            const data = await response.json();

            // Remove typing indicator
            this.hideTypingIndicator();

            // Add assistant response
            this.addMessage(data.response, 'assistant');

            // Update conversation history
            this.conversationHistory.push({
                role: 'user',
                content: message
            });
            this.conversationHistory.push({
                role: 'assistant',
                content: data.response
            });

            // Keep only last 10 messages
            if (this.conversationHistory.length > 20) {
                this.conversationHistory = this.conversationHistory.slice(-20);
            }

        } catch (error) {
            console.error('Error sending message:', error);
            this.hideTypingIndicator();
            this.addMessage('Xin lỗi, đã có lỗi xảy ra. Vui lòng thử lại.', 'assistant');
        }
    }

    addMessage(text, role) {
        const messagesContainer = document.getElementById('chatMessages');
        const messageDiv = document.createElement('div');
        messageDiv.className = `message ${role}`;
        
        const now = new Date();
        const time = now.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });

        messageDiv.innerHTML = `
            <div class="message-avatar">
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                    ${role === 'user' 
                        ? '<path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z"/>'
                        : '<path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z"/><circle cx="12" cy="12" r="3"/>'
                    }
                </svg>
            </div>
            <div class="message-bubble">
                <p class="message-text">${this.escapeHtml(text)}</p>
                <div class="message-time">${time}</div>
            </div>
        `;

        messagesContainer.appendChild(messageDiv);
        this.scrollToBottom();
    }

    showTypingIndicator() {
        if (this.isTyping) return;
        this.isTyping = true;

        const messagesContainer = document.getElementById('chatMessages');
        const typingDiv = document.createElement('div');
        typingDiv.className = 'message typing-message';
        typingDiv.innerHTML = `
            <div class="message-avatar">
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                    <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z"/>
                    <circle cx="12" cy="12" r="3"/>
                </svg>
            </div>
            <div class="typing-indicator">
                <div class="typing-dot"></div>
                <div class="typing-dot"></div>
                <div class="typing-dot"></div>
            </div>
        `;

        messagesContainer.appendChild(typingDiv);
        this.scrollToBottom();
    }

    hideTypingIndicator() {
        this.isTyping = false;
        const typingMessage = document.querySelector('.typing-message');
        if (typingMessage) {
            typingMessage.remove();
        }
    }

    scrollToBottom() {
        const chatContent = document.getElementById('chatContent');
        chatContent.scrollTop = chatContent.scrollHeight;
    }

    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Vapi Integration
    initVapi() {
        let retryCount = 0;
        const maxRetries = 30; // Tăng lên 30 lần (15 giây)
        
        // Đợi Vapi SDK load
        const checkVapi = () => {
            retryCount++;
            if (typeof Vapi !== 'undefined') {
                try {
                    this.vapiClient = new Vapi('13c18feb-606d-48e0-9e12-7a92fbb7e2aa'); // Public key
                    
                    // Listen to Vapi events
                    this.vapiClient.on('call-start', () => {
                        console.log('Call started');
                        this.updateVoiceStatus('Đang kết nối...', true);
                    });

                    this.vapiClient.on('call-end', () => {
                        console.log('Call ended');
                        this.isCallActive = false;
                        this.stopCallTimer();
                        this.updateVoiceStatus('Cuộc gọi đã kết thúc', false);
                        document.getElementById('voiceControls').style.display = 'none';
                    });

                    this.vapiClient.on('speech-start', () => {
                        document.getElementById('voiceAvatar').classList.add('speaking');
                    });

                    this.vapiClient.on('speech-end', () => {
                        document.getElementById('voiceAvatar').classList.remove('speaking');
                    });

                    this.vapiClient.on('error', (error) => {
                        console.error('Vapi error:', error);
                        this.updateVoiceStatus('Lỗi kết nối. Vui lòng thử lại.', false);
                        this.isCallActive = false;
                    });
                    
                    console.log('✅ Vapi SDK initialized successfully');
                } catch (error) {
                    console.error('❌ Error initializing Vapi:', error);
                }
            } else {
                if (retryCount < maxRetries) {
                    console.warn(`⏳ Vapi SDK not loaded yet, retrying... (${retryCount}/${maxRetries})`);
                    setTimeout(checkVapi, 500);
                } else {
                    console.error('❌ Vapi SDK failed to load after maximum retries. CDN might be blocked or slow.');
                    console.error('💡 Try: 1) Check internet connection, 2) Reload page, 3) Check if CDN is accessible');
                }
            }
        };
        
        checkVapi();
    }

    async startVoiceCall() {
        if (!this.vapiClient) {
            // Hiển thị thông báo thân thiện hơn
            this.updateVoiceStatus('Đang tải SDK...', false);
            
            // Thử khởi tạo lại Vapi
            this.initVapi();
            
            // Đợi một chút rồi thử lại
            setTimeout(() => {
                if (!this.vapiClient) {
                    this.updateVoiceStatus('Không thể kết nối. Vui lòng tải lại trang.', false);
                    console.error('Vapi SDK is not available');
                } else {
                    // Thử gọi lại
                    this.startVoiceCall();
                }
            }, 1000);
            return;
        }

        try {
            this.updateVoiceStatus('Đang kết nối...', false);

            const assistantId = 'bc0de302-5c28-4079-92b6-243296d8fd1e';
            
            await this.vapiClient.start(assistantId);

            this.isCallActive = true;
            this.startCallTimer();
            this.updateVoiceStatus('Đang gọi...', true);
            document.getElementById('voiceControls').style.display = 'flex';

        } catch (error) {
            console.error('Error starting call:', error);
            this.updateVoiceStatus('Không thể kết nối. Thử lại.', false);
        }
    }

    endVoiceCall() {
        if (this.vapiClient && this.isCallActive) {
            this.vapiClient.stop();
            this.isCallActive = false;
            this.stopCallTimer();
            this.updateVoiceStatus('Nhấn để bắt đầu cuộc gọi', false);
            document.getElementById('voiceControls').style.display = 'none';
        }
    }

    toggleMute() {
        if (!this.vapiClient || !this.isCallActive) return;

        this.isMuted = !this.isMuted;
        this.vapiClient.setMuted(this.isMuted);

        const muteBtn = document.getElementById('muteBtn');
        if (this.isMuted) {
            muteBtn.classList.add('muted');
        } else {
            muteBtn.classList.remove('muted');
        }
    }

    startCallTimer() {
        this.callDuration = 0;
        this.callTimer = setInterval(() => {
            this.callDuration++;
            this.updateCallDuration();
        }, 1000);
    }

    stopCallTimer() {
        if (this.callTimer) {
            clearInterval(this.callTimer);
            this.callTimer = null;
        }
        this.callDuration = 0;
        document.getElementById('voiceDuration').textContent = '00:00';
    }

    updateCallDuration() {
        const minutes = Math.floor(this.callDuration / 60);
        const seconds = this.callDuration % 60;
        const formatted = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
        document.getElementById('voiceDuration').textContent = formatted;
    }

    updateVoiceStatus(status, showControls) {
        document.getElementById('voiceStatus').textContent = status;
    }

    getVapiFunctions() {
        // Define functions mà Vapi có thể call
        return [
            {
                name: "get_products",
                description: "Lấy danh sách sản phẩm",
                parameters: {
                    type: "object",
                    properties: {
                        limit: { type: "number", description: "Số lượng sản phẩm cần lấy" }
                    }
                },
                url: `${window.location.origin}/api/Assistant/vapi/tools`
            },
            {
                name: "search_products",
                description: "Tìm kiếm sản phẩm theo tên hoặc mã",
                parameters: {
                    type: "object",
                    properties: {
                        keyword: { type: "string", description: "Từ khóa tìm kiếm" }
                    },
                    required: ["keyword"]
                },
                url: `${window.location.origin}/api/Assistant/vapi/tools`
            },
            {
                name: "get_orders",
                description: "Lấy danh sách đơn hàng",
                parameters: {
                    type: "object",
                    properties: {
                        limit: { type: "number" },
                        status: { type: "string", description: "Trạng thái đơn hàng" }
                    }
                },
                url: `${window.location.origin}/api/Assistant/vapi/tools`
            }
        ];
    }
}

// Khởi tạo chat bubble khi trang load
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initChatBubble);
} else {
    // DOM đã load sẵn
    initChatBubble();
}

function initChatBubble() {
    // Đợi thêm một chút để đảm bảo các script external đã load
    setTimeout(() => {
        try {
            window.chatBubble = new ChatBubble();
            console.log('Chat bubble initialized successfully');
        } catch (error) {
            console.error('Error initializing chat bubble:', error);
        }
    }, 100);
}
