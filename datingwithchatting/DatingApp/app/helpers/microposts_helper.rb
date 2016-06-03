module MicropostsHelper
	def find_current_micropost(id)
	 @post_id = id
	 Micropost.find_by(id: id);
	end

	def current_micropost
	 @current_microposts= Micropost.find_by(id: @post_id);
	end
end
