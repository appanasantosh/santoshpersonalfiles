module ContestHelper
	def select_picture(uid)
        @pictureSelection = PictureSelection.new
        
        @pictureSelection.user_id = uid

        @pictureSelection.save         
    end

    def unselect_picture(uid)
        @pictureSelection = PictureSelection.find_by_user_id((uid))

        if(!@pictureSelection.nil?)

            @pictureSelection.destroy
            
        end
    end

end
