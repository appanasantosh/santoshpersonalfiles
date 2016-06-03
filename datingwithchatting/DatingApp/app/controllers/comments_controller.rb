class CommentsController < ApplicationController
	before_action :signed_in_user, only: [:create, :destroy]

	def create
		#@mp = Micropost.find(id: @current_micropost.id)

		@post = Micropost.find(params[:comment][:post_id])

		@comment = @post.comments.build(comment_params)
		
		@comment.user = @current_user

		#@comment = current_micropost.comments.build(comment_params)

		if @comment.save
			flash[:success] = "Comment posted!"
			redirect_to root_url
		else
			flash[:error] = "Comment not posted!"
			redirect_to root_url
		end
	end

	def destroy
	end

	private

	def comment_params
		params.require(:comment).permit(:content)
	end
end
