class StaticpagesController < ApplicationController
  def home
  	if signed_in?
      @micropost  = current_user.microposts.build
      @comment = @micropost.comments.build
      @micropost_items = current_user.all_posts.paginate(page: params[:page])

    end
  end

  def contest
    
    @visitor = Visitor.new
  	@contests = Contest.all.paginate(page: params[:page])
  end

end
