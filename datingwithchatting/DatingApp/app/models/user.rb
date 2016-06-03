class User < ActiveRecord::Base
	has_many :profileimages
	has_many :authorizations
  has_many :picture_selections
	has_many :conversations, :foreign_key => :sender_id
  has_many :microposts, dependent: :destroy
  has_many :comments, dependent: :destroy
  has_many :contest_results, :foreign_key => :User_id 

	validates :name, presence: true, length: {maximum: 50}
	VALID_EMAIL_REGEX = /\A[\w+\-.]+@[a-z\d\-.]+\.[a-z]+\z/i
	
	validates :email, presence: true, format: {with: VALID_EMAIL_REGEX}, uniqueness: {case_sensitive: false}
	validates :password, format: {with: /^(?=.*[a-zA-Z])(?=.*[0-9]).{6,}$/, message:"must be at least 6 characters and include one number and one letter.",:multiline => true }
	before_save {self.email = email.downcase}
	before_create :create_remember_token
	has_secure_password
	has_attached_file :userimage, validate_media_type: false 

	acts_as_messageable
	
	
	validates_attachment_content_type :userimage, :content_type => /\Aimage\/.*\Z/

  # Validate filename
  validates_attachment_file_name :userimage, :matches => [/png\Z/, /jpe?g\Z/]

  # Explicitly do not validate
  do_not_validate_attachment_file_type :userimage


  def mailboxer_name
  	self.name
  end

  def mailboxer_email(object)
  	self.email
  end


  def all_posts
    Micropost.all_microposts
  end

  



  private

  def User.new_remember_token
  	SecureRandom.urlsafe_base64
  end

  def User.digest(token)
  	Digest::SHA1.hexdigest(token.to_s)
  end


  def create_remember_token
  	self.remember_token = User.digest(User.new_remember_token)
  end

  def self.search(search)
  	if search
  		where('name Like ?' "%#{search}%")
  	else
  		scoped
  	end
  end
end
