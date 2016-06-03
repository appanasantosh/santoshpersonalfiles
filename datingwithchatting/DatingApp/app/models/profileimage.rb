class Profileimage < ActiveRecord::Base
	belongs_to :user
	default_scope -> {order('created_at DESC')}
	validates :user_id, presence: true

	has_attached_file :image, validate_media_type: false ,
	
	processors: [:transcoder]
	
   
	validates_attachment_content_type :image, :content_type => ['image/png', 'image/jpg', 'image/jpeg'] , :if => :is_type_of_image?
    validates_attachment_content_type :image, :content_type => ['video/mp4'], :message => 'Sorry right now we are supporting mp4', :if => :is_type_of_video?
	
  

  # Explicitly do not validate



	protected

	def is_type_of_video?
    	image.content_type =~ %r(video)
  	end

  	def is_type_of_image?
    	image.content_type =~ %r(image)
  	end
end
