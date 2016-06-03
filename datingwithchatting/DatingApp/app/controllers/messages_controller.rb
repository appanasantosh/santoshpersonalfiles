class MessagesController < ApplicationController
	  before_filter :signed_in_user

  def create
    @conversation = Conversation.find(params[:conversation_id])
    @message = @conversation.messages.build(message_params)
    @message.user_id = current_user.id
    @message.save!

    @path = conversation_path(@conversation)
    
  end

  private

  def message_params
    params.require(:message).permit(:body)
  end
  def signed_in_user
      unless signed_in?
        store_location
        redirect_to signin_url, notice: "Please sign in."
      end
  end
end
