class Contest < ActiveRecord::Base
	default_scope -> { order('created_at DESC') }
	validates :name, presence: true, length: { maximum: 1400 }
end
