class Visitor < ActiveRecord::Base
	validates :name, presence: true, length: {maximum: 50}
	VALID_EMAIL_REGEX = /\A[\w+\-.]+@[a-z\d\-.]+\.[a-z]+\z/i
	
	validates :email, presence: true, format: {with: VALID_EMAIL_REGEX}, uniqueness: {case_sensitive: false}
	validates :password, format: {with: /^(?=.*[a-zA-Z])(?=.*[0-9]).{6,}$/, message:"must be at least 6 characters and include one number and one letter.",:multiline => true }
	before_save {self.email = email.downcase}
	has_secure_password
end
