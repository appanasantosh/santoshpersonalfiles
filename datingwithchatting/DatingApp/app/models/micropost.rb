class Micropost < ActiveRecord::Base
	belongs_to :user
	has_many :comments, dependent: :destroy
	default_scope -> { order('created_at DESC') }
	validates :content, presence: true, length: { maximum: 1400 }
	validates :user_id, presence: true


	def self.all_microposts
		all_posts = Micropost.all
	end

	def user_comments(post_id)
		@micropost = Micropost.find(post_id)
		@comment_items = @micropost.comments
	end


end
