<%@ Page Title="" Language="C#" MasterPageFile="~/layouts/PrivateLayout.Master" AutoEventWireup="true" CodeBehind="TrendyChat.aspx.cs" Inherits="TrendBlend.pages.TrendyChat" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/styles/TrendyChat/styles.css" rel="stylesheet" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="chat_container">
        <div class="chat_messages" id="chatMessages">
            <div class="messages_wrapper" id="messagesWrapper">
                <!-- Initial welcome message -->
                <div class="message ai_message">
                    <div class="message_content">
                        <p>Hi! I'm TrendyAI. I can help you find the perfect outfit from your wardrobe. What are you dressing for today?</p>
                    </div>
                </div>
                <!-- Add this invisible element to ensure we can scroll to the bottom -->
                <div class="scroll_anchor" id="scrollAnchor"></div>
            </div>
        </div>

        <div class="chat_input_container">
            <input type="text" id="txtEventType" class="chat_input"
                placeholder="Type your event (e.g., wedding, job interview, dinner date)..." />
            <button type="button" id="btnSendMessage" class="send_btn">
                <i class="fa fa-paper-plane"></i>
            </button>
        </div>

        <div id="loadingIndicator" class="typing_indicator" style="display: none;">
            <span></span>
            <span></span>
            <span></span>
        </div>
    </div>

    <!-- Save Blend Modal -->
    <div id="saveBlendModal" class="modal_overlay" style="display: none;">
        <div class="modal_content">
            <h3>Save as Favorite Blend</h3>
            <p>Give your outfit blend a name:</p>
            <input type="text" id="blendNameInput" class="blend_name_input" placeholder="Enter blend name...">
            <div class="modal_buttons">
                <button type="button" id="cancelSaveBlend" class="modal_button cancel_button">Cancel</button>
                <button type="button" id="confirmSaveBlend" class="modal_button confirm_button">Save</button>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        $(document).ready(function () {
            const $chatMessages = $('#chatMessages');
            const $messagesWrapper = $('#messagesWrapper');
            const $eventInput = $('#txtEventType');
            const $sendBtn = $('#btnSendMessage');
            const $loadingIndicator = $('#loadingIndicator');
            const $saveBlendModal = $('#saveBlendModal');
            const $blendNameInput = $('#blendNameInput');
            const $cancelSaveBlend = $('#cancelSaveBlend');
            const $confirmSaveBlend = $('#confirmSaveBlend');

            const username = '<%= Session["UserName"] %>';
            let currentOutfitData = null;

            let chatHistory = [];

            function saveChatHistory() {
                // Limit history to last 50 messages to prevent localStorage size issues
                if (chatHistory.length > 50) {
                    chatHistory = chatHistory.slice(-50);
                }
                localStorage.setItem(`chatHistory_${username}`, JSON.stringify(chatHistory));
            }

            // Function to load chat history from localStorage
            function loadChatHistory() {
                try {
                    const savedHistory = localStorage.getItem(`chatHistory_${username}`);
                    if (savedHistory) {
                        chatHistory = JSON.parse(savedHistory);

                        // Only clear and rebuild if we have history
                        if (chatHistory.length > 0) {
                            // Clear existing welcome message
                            $messagesWrapper.empty();

                            // Add scroll anchor back
                            $messagesWrapper.append('<div class="scroll_anchor" id="scrollAnchor"></div>');

                            // Rebuild the chat from history
                            chatHistory.forEach(message => {
                                if (message.type === 'user') {
                                    addUserMessage(message.content, false);
                                } else if (message.type === 'ai') {
                                    addAIMessage(message.content, false);
                                } else if (message.type === 'outfit') {
                                    addOutfitRecommendation(message.content, false);
                                }
                            });

                            // Scroll to bottom after loading history
                            scrollToBottom();
                        }
                    }
                } catch (error) {
                    console.error('Error loading chat history:', error);
                    // If there's an error, just start fresh
                    chatHistory = [];
                }
            }

            // Updated scrollToBottom function in TrendyChat.aspx
            function scrollToBottom() {
                // Force a reflow by accessing offsetHeight
                const dummy = $chatMessages[0].offsetHeight;

                // Use requestAnimationFrame for smoother scrolling after DOM updates
                requestAnimationFrame(() => {
                    $chatMessages.scrollTop($chatMessages[0].scrollHeight);
                });
            }

            function addUserMessage(message, saveToHistory = true) {
                const userMessage = `
        <div class="message user_message">
            <div class="message_content">
                <p>${message}</p>
            </div>
        </div>
    `;

                // Insert before the anchor
                $(userMessage).insertBefore('#scrollAnchor');

                if (saveToHistory) {
                    chatHistory.push({ type: 'user', content: message });
                    saveChatHistory();
                }

                // Force scroll update
                scrollToBottom();
            }

            // Add AI message to the chat
            function addAIMessage(message, saveToHistory = true) {
                const aiMessage = `
        <div class="message ai_message">
            <div class="message_content">
                <p>${message}</p>
            </div>
        </div>
    `;

                // Insert before the anchor
                $(aiMessage).insertBefore('#scrollAnchor');

                if (saveToHistory) {
                    chatHistory.push({ type: 'ai', content: message });
                    saveChatHistory();
                }

                // Force scroll update
                scrollToBottom();
            }

            // Add an outfit recommendation to the chat
            function addOutfitRecommendation(recommendation, saveToHistory = true) {
                let recommendationHtml = `
        <div class="message ai_message">
            <div class="message_content outfit_recommendation">
                <h2>${recommendation.outfitName}</h2>
                <p class="outfit_description">${recommendation.description}</p>
                <div class="outfit_items">`;

                // We'll populate the items later with AJAX calls
                recommendationHtml += `</div>
                <div class="outfit_tips">
                    <h3>Styling Tips</h3>
                    <p>${recommendation.stylingTips}</p>
                </div>
                <div class="outfit_actions">
                    <button class="save_outfit_btn" data-outfit='${JSON.stringify(recommendation)}'>
                        <i class="fa fa-heart"></i> Save as Favorite Blend
                    </button>
                </div>
            </div>
        </div>Sa
    `;

                // Insert before the anchor
                $(recommendationHtml).insertBefore('#scrollAnchor');

                // Save to history just once here, outside the forEach loop
                if (saveToHistory) {
                    chatHistory.push({ type: 'outfit', content: recommendation });
                    saveChatHistory();
                }

                // Now load the apparel items
                const $lastOutfitItems = $('.outfit_items').last();

                // Track loading state for each item
                let itemsToLoad = recommendation.items.length;
                let itemsLoaded = 0;

                recommendation.items.forEach(async item => {
                    try {
                        const response = await $.ajax({
                            url: '/services/ApparelService.asmx/GetApparelDetails',
                            type: 'POST',
                            data: JSON.stringify({ apparelId: item.id }),
                            contentType: 'application/json',
                            dataType: 'json'
                        });

                        const apparel = response.d;
                        if (apparel) {
                            const itemHtml = `
                    <div class="outfit_item" onclick="window.location.href='/pages/Apparel.aspx?id=${apparel.ApparelId}'">
                        <div class="item_image_container">
                            <img src="${apparel.ImageUrl}" alt="${apparel.Name}" />
                        </div>
                        <div class="item_details">
                            <h3>${apparel.Name}</h3>
                            <p><strong>${item.type}</strong></p>
                            <p class="item_reason">${item.reason}</p>
                        </div>
                    </div>`;
                            $lastOutfitItems.append(itemHtml);

                            itemsLoaded++;
                            if (itemsLoaded === itemsToLoad) {
                                // All items loaded, now scroll
                                scrollToBottom();
                            }
                        }
                    } catch (error) {
                        console.error(`Error fetching apparel ${item.id}:`, error);
                        itemsLoaded++;
                        if (itemsLoaded === itemsToLoad) {
                            // All items attempted, now scroll
                            scrollToBottom();
                        }
                    }
                });

                // Initial scroll after adding the message structure
                scrollToBottom();
            }

            // Show loading indicator
            function showLoading() {
                $loadingIndicator.show();
                scrollToBottom();
            }

            // Hide loading indicator
            function hideLoading() {
                $loadingIndicator.hide();
            }

            // Show save blend modal
            function showSaveBlendModal(outfitData) {
                currentOutfitData = outfitData;
                $blendNameInput.val(outfitData.outfitName || "My Outfit");
                $saveBlendModal.fadeIn(200);
                $blendNameInput.focus();
            }

            // Hide save blend modal
            function hideSaveBlendModal() {
                $saveBlendModal.fadeOut(200);
                currentOutfitData = null;
            }

            // Process user input and get recommendation
            async function processUserInput() {
                const eventType = $eventInput.val().trim();
                if (!eventType) return;

                // Clear input
                $eventInput.val('');

                // Add user message to chat
                addUserMessage(eventType);

                // Show loading
                showLoading();

                try {
                    // First add a thinking message
                    addAIMessage("Looking through your wardrobe for the perfect outfit...");

                    // Make API request
                    const response = await $.ajax({
                        url: '/services/ApparelService.asmx/GetOutfitRecommendation',
                        type: 'POST',
                        data: JSON.stringify({
                            eventType: eventType,
                            username: username
                        }),
                        contentType: 'application/json',
                        dataType: 'json'
                    });

                    // Process response
                    const geminiResponse = response.d;
                    if (!geminiResponse || !geminiResponse.candidates || !geminiResponse.candidates.length) {
                        throw new Error('Invalid response from AI service');
                    }

                    const responseText = geminiResponse.candidates[0].content.parts[0].text;
                    const jsonMatch = responseText.match(/\{[\s\S]*\}/);

                    if (!jsonMatch) {
                        throw new Error('Could not extract recommendation from AI response');
                    }

                    // Parse and display recommendation
                    const recommendation = JSON.parse(jsonMatch[0]);
                    addAIMessage(`I've found a perfect outfit for your ${eventType}:`);
                    addOutfitRecommendation(recommendation);

                } catch (error) {
                    console.error('Error:', error);
                    addAIMessage(`Sorry, I encountered an error: ${error.message}. Please try again.`);
                } finally {
                    hideLoading();
                }
            }

            // Save outfit as blend
            async function saveOutfitAsBlend() {
                if (!currentOutfitData || !$blendNameInput.val().trim()) {
                    return;
                }

                const blendName = $blendNameInput.val().trim();

                try {
                    // First create the blend
                    const createResponse = await $.ajax({
                        url: '/services/ApparelService.asmx/CreateBlend',
                        type: 'POST',
                        data: JSON.stringify({
                            username: username,
                            blendName: blendName
                        }),
                        contentType: 'application/json',
                        dataType: 'json'
                    });

                    if (createResponse.d) {
                        const blendId = createResponse.d;

                        // Add each item to the blend
                        for (const item of currentOutfitData.items) {
                            await $.ajax({
                                url: '/services/ApparelService.asmx/AddApparelToBlend',
                                type: 'POST',
                                data: JSON.stringify({
                                    blendId: blendId,
                                    apparelId: item.id
                                }),
                                contentType: 'application/json',
                                dataType: 'json'
                            });
                        }

                        // Hide modal
                        hideSaveBlendModal();

                        // Add success message
                        addAIMessage(`Great! I've saved "${blendName}" to your favorite blends.`);

                        // Disable the save button on the outfit
                        $(`.save_outfit_btn[data-outfit]`).filter(function () {
                            try {
                                const btnData = JSON.parse($(this).attr('data-outfit'));
                                return btnData.outfitName === currentOutfitData.outfitName;
                            } catch (e) {
                                return false;
                            }
                        }).prop('disabled', true).html('<i class="fa fa-check"></i> Saved!');

                        currentOutfitData = null;
                    }
                } catch (error) {
                    console.error('Error saving blend:', error);
                    addAIMessage(`Sorry, I couldn't save this outfit: ${error.message}`);
                    hideSaveBlendModal();
                }
            }

            // Make sure the chat scrolls to bottom on page load
            scrollToBottom();

            // Event handlers
            $sendBtn.click(processUserInput);

            $eventInput.keypress(function (e) {
                if (e.which === 13) { // Enter key
                    processUserInput();
                }
            });

            // Handle save outfit button click
            $(document).on('click', '.save_outfit_btn', function () {
                try {
                    const outfitData = JSON.parse($(this).attr('data-outfit'));
                    showSaveBlendModal(outfitData);
                } catch (error) {
                    console.error('Error parsing outfit data', error);
                    addAIMessage("Sorry, I couldn't prepare this outfit for saving. Please try again.");
                }
            });

            // Save blend modal controls
            $cancelSaveBlend.click(hideSaveBlendModal);
            $confirmSaveBlend.click(saveOutfitAsBlend);
            $blendNameInput.keypress(function (e) {
                if (e.which === 13) { // Enter key
                    saveOutfitAsBlend();
                }
            });

            // Close modal when clicking outside
            $saveBlendModal.click(function (e) {
                if (e.target === this) {
                    hideSaveBlendModal();
                }
            });

            // Initial focus
            $eventInput.focus();

            $(window).on('resize', scrollToBottom);

            // Fix for mobile browsers that might resize on input focus
            $eventInput.on('focus', function () {
                setTimeout(scrollToBottom, 300);
            });

            // Initial scroll
            setTimeout(scrollToBottom, 100);

            loadChatHistory();
        });
    </script>
</asp:Content>
