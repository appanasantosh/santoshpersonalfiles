class ContestResult < ActiveRecord::Base
	belongs_to :user, :foreign_key => :User_id
	default_scope -> {order('created_at DESC')}
	validates :User_id, presence: true
end
