<%@ Page Title="" Language="C#" MasterPageFile="~/layouts/PrivateLayout.Master" AutoEventWireup="true" CodeBehind="TrendyChat.aspx.cs" Inherits="TrendBlend.pages.TrendyChat" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/styles/TrendyChat/styles.css" rel="stylesheet" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="chat_container">
        <div class="chat_messages" id="chatMessages">
            <div class="messages_wrapper messages_auto_scroll" id="messagesWrapper">
                <!-- Initial welcome message -->
                <div class="message ai_message">
                    <div class="message_content">
                        <p>
                            Hi! I'm TrendyAI. I can help you find the perfect outfit from your wardrobe.<br />
                            <br />
                            What are you dressing for today? 👕👗
                       
                        </p>
                    </div>
                </div>
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
                        }
                    }
                } catch (error) {
                    console.error('Error loading chat history:', error);
                    // If there's an error, just start fresh
                    chatHistory = [];
                }
            }

            function addUserMessage(message, saveToHistory = true) {
                const userMessage = `
                    <div class="message user_message">
                        <div class="message_content">
                            <p>${message}</p>
                        </div>
                    </div>
                `;

                // Append to the messages wrapper
                $messagesWrapper.append(userMessage);

                if (saveToHistory) {
                    chatHistory.push({ type: 'user', content: message });
                    saveChatHistory();
                }
            }

            // Add AI message to the chat
            function addAIMessage(message, saveToHistory = true, isError = false) {
                const messageClass = isError ? "ai_message error_message" : "ai_message";

                const aiMessage = `
                    <div class="message ${messageClass}">
                        <div class="message_content">
                            <p>${message}</p>
                        </div>
                    </div>
                `;

                $messagesWrapper.append(aiMessage);

                if (saveToHistory && !isError) {
                    chatHistory.push({ type: 'ai', content: message });
                    saveChatHistory();
                }
            }

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
                                <button class="save_outfit_btn" data-outfit-id="${btoa(JSON.stringify(recommendation))}">
                                    <i class="fa fa-heart"></i> Save as Favorite Blend
                                </button>
                            </div>
                        </div>
                    </div>
                `;

                // Append to messages wrapper
                $messagesWrapper.append(recommendationHtml);

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
                        }
                    } catch (error) {
                        console.error(`Error fetching apparel ${item.id}:`, error);
                        itemsLoaded++;
                    }
                });
            }

            // Show loading indicator
            function showLoading() {
                $loadingIndicator.show();
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
                    // Show error but don't save to history
                    addAIMessage(`Sorry, I encountered an error: ${error.message}. Please try again.`, false, true);
                } finally {
                    hideLoading();
                }
            }

async function saveOutfitAsBlend() {
    if (!currentOutfitData || !$blendNameInput.val().trim()) {
        return;
    }

    const blendName = $blendNameInput.val().trim();
    
    // Create a local copy of the outfit data to prevent issues if currentOutfitData becomes null
    const outfitData = {...currentOutfitData};

    try {
        // Show a temporary saving message
        const $savingMsg = $('<div class="message ai_message saving_message"><div class="message_content"><p>Saving your blend...</p></div></div>');
        $messagesWrapper.append($savingMsg);

        // Safely handle various data types
        let cleanDescription = "";
        if (outfitData.description) {
            cleanDescription = typeof outfitData.description === 'string' 
                ? outfitData.description.substring(0, 500) 
                : String(outfitData.description).substring(0, 500);
        }
        
        let cleanStylingTips = "";
        if (outfitData.stylingTips) {
            cleanStylingTips = typeof outfitData.stylingTips === 'string' 
                ? outfitData.stylingTips.substring(0, 500) 
                : String(outfitData.stylingTips).substring(0, 500);
        }

        // First create the blend with description and styling tips
        const createResponse = await $.ajax({
            url: '/services/ApparelService.asmx/CreateBlend',
            type: 'POST',
            data: JSON.stringify({
                username: username,
                blendName: blendName,
                description: cleanDescription,
                wearingSuggestion: cleanStylingTips
            }),
            contentType: 'application/json',
            dataType: 'json',
            timeout: 30000 // 30 second timeout
        });

        if (createResponse.d) {
            const blendId = createResponse.d;
            let successCount = 0;
            let failedItems = [];

            // Make sure outfitData.items exists before proceeding
            const items = outfitData.items || [];
            const totalItems = items.length;

            // Add each item to the blend
            for (const item of items) {
                try {
                    // Ensure we have a valid ID
                    const apparelId = parseInt(item.id, 10);
                    if (isNaN(apparelId) || apparelId <= 0) {
                        console.warn(`Skipping invalid apparel ID: ${item.id}`);
                        continue;
                    }
                    
                    const addResponse = await $.ajax({
                        url: '/services/ApparelService.asmx/AddApparelToBlend',
                        type: 'POST',
                        data: JSON.stringify({
                            blendId: blendId,
                            apparelId: apparelId
                        }),
                        contentType: 'application/json',
                        dataType: 'json',
                        timeout: 10000 // 10 second timeout per item
                    });
                    
                    if (addResponse.d === true) {
                        successCount++;
                    }
                } catch (itemError) {
                    failedItems.push(item.id);
                    console.error(`Failed to add item ${item.id} to blend:`, itemError);
                }
            }

            // Remove the temporary saving message
            $savingMsg.remove();
            
            // Save the current state before clearing it
            const savedOutfitData = JSON.stringify(outfitData);
            
            // Hide modal (this will set currentOutfitData to null)
            hideSaveBlendModal();

            // Add success message based on how many items were added successfully
            if (totalItems === 0) {
                addAIMessage(`I've saved "${blendName}" to your favorite blends.`);
            } else if (successCount === totalItems) {
                addAIMessage(`Great! I've saved "${blendName}" to your favorite blends with all ${successCount} items.`);
            } else if (successCount > 0) {
                addAIMessage(`I've saved "${blendName}" to your favorite blends, but only ${successCount} out of ${totalItems} items were added successfully.`);
            } else {
                addAIMessage(`I've created "${blendName}" in your favorite blends, but couldn't add any items to it. Please try again.`, false, true);
            }

            // Disable the save button using the saved data (since currentOutfitData is now null)
            try {
                const encodedOutfitData = btoa(savedOutfitData);
                $(`.save_outfit_btn[data-outfit-id="${encodedOutfitData}"]`)
                    .prop('disabled', true)
                    .html('<i class="fa fa-check"></i> Saved!');
            } catch (btnError) {
                console.error('Error updating save button:', btnError);
            }
        }
    } catch (error) {
        console.error('Error saving blend:', error);
        // Remove any temporary saving message
        $('.saving_message').remove();
        
        // Show detailed error message
        let errorMessage = "Sorry, I couldn't save this outfit.";
        if (error.responseText) {
            try {
                const errorDetails = JSON.parse(error.responseText);
                if (errorDetails.Message) {
                    errorMessage += ` Error: ${errorDetails.Message}`;
                }
            } catch (e) {
                errorMessage += " Please try again later.";
            }
        } else if (error.message) {
            errorMessage += ` Error: ${error.message}`;
        }
        
        addAIMessage(errorMessage, false, true);
        hideSaveBlendModal();
    }
}

                   // Handle save outfit button click - prevent default behavior to avoid page reloads
            $(document).on('click', '.save_outfit_btn', function (e) {
                // Prevent default button behavior that might cause page reload
                e.preventDefault();
                e.stopPropagation();

                try {
                    // Get the base64 encoded data and decode it
                    const encodedData = $(this).attr('data-outfit-id');
                    const outfitData = JSON.parse(atob(encodedData));
                    showSaveBlendModal(outfitData);
                } catch (error) {
                    console.error('Error parsing outfit data', error);
                    addAIMessage("Sorry, I couldn't prepare this outfit for saving. Please try again.", false, true);
                }

                return false; // Prevent event bubbling
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

            // Make sure input is always visible when keyboard appears (mobile)
            $eventInput.on('focus', function() {
                // Add a slight delay to account for keyboard appearance
                setTimeout(function() {
                    $eventInput[0].scrollIntoView(false);
                }, 300);
            });

            // Enable auto-hiding of the loading indicator
            $loadingIndicator.on('show', function() {
                // Auto-hide after 30 seconds to prevent it getting stuck
                setTimeout(function() {
                    if ($loadingIndicator.is(':visible')) {
                        hideLoading();
                        addAIMessage("It's taking longer than expected. Please try again if you don't see a response.", false, true);
                    }
                }, 30000);
            });

            // Clear chat history function
            function clearChatHistory() {
                if (confirm("Are you sure you want to clear your chat history?")) {
                    localStorage.removeItem(`chatHistory_${username}`);
                    location.reload();
                }
            }

            // Add a clear history button
            const $clearHistoryBtn = $('<button class="clear_history_btn"><i class="fa fa-trash"></i> Clear History</button>');
            $('.chat_container').append($clearHistoryBtn);
            $clearHistoryBtn.on('click', clearChatHistory);

            $(document).keypress(function(e) {
    // Check if Enter key was pressed
    if (e.which === 13) { 
        // If the focused element is the event input field
        if (document.activeElement === $eventInput[0]) {
            // Only process if there's text in the input
            if ($eventInput.val().trim()) {
                e.preventDefault(); // Prevent default Enter key behavior
                processUserInput();
                return false;
            }
        } 
        // If the focused element is the blend name input
        else if (document.activeElement === $blendNameInput[0]) {
            // Only process if there's text in the input
            if ($blendNameInput.val().trim()) {
                e.preventDefault(); // Prevent default Enter key behavior
                saveOutfitAsBlend();
                return false;
            }
        }
    }
});

// Remove the individual keypress handlers and replace with the above global handler
$eventInput.off('keypress');
$blendNameInput.off('keypress');

// Update event handlers to use the consolidated approach
$sendBtn.click(function() {
    if ($eventInput.val().trim()) {
        processUserInput();
    }
});
            
            // Load chat history
            loadChatHistory();
        });
    </script>
</asp:Content>
