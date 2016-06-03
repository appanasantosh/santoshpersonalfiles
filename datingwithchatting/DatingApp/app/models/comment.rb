class Comment < ActiveRecord::Base
	belongs_to :micropost
	belongs_to :user
	default_scope -> { order('created_at DESC') }
	validates :content, presence: true, length: { maximum: 1400 }
	validates :micropost_id, presence: true

	def self.comments_by_posts(post_id)
		Comment.find_by(micropost_id: post_id)
	end

	
	
end
