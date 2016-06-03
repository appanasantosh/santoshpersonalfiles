class RegisterController < ApplicationController

	def register
		if logged_in?
			redirect_to :action => 'main', :id => current_user.id
		else
			@user = User.new
		end
		
	end

	def CreateUser
		@user = User.create(reg_params)
		if @user.save
			redirect_to :action => 'signin' 
		else
			render :register
		end
	end

	def WeeklyGoalFeedback

		render :layout => false

	end

	def dailygoals

		@usergoals = Goal.where("userid = :userid and Date(DoToday) = :date", { userid: params[:id], date: Date.today })
		@pendingcount = Goal.where("userid = :userid and status != :status and Date(created_at)!= :date", { userid: params[:id], status: 'Done' , date: Date.today}).count

		render :layout => false

	end


	def loadondate
		 #@usergoals = Goal.where(userid: current_user.id)
		 if(params[:loadmode] == "1")
		   	@usergoals = Goal.where("userid = :userid and Date(DoToday) = :date", { userid: current_user.id, date: params[:selecteddate] })
		 else if(params[:loadmode] == "2")
		 	@usergoals = Goal.where("userid = :userid and Date(created_at) = :date", { userid: current_user.id, date: params[:selecteddate] })
		 end
		end
		respond_to do |format|
			format.html 
			format.json { render :json => @usergoals}
		end
	end

	def main
		@user = User.find_by(id: params[:id])
	#	@usergoals = Goal.where("userid = :userid and Date(DoToday) = :date", { userid: params[:id], date: Date.today })
	#	@pendingcount = Goal.where("userid = :userid and status != :status and Date(created_at)!= :date", { userid: params[:id], status: 'Done' , date: Date.today}).count
		
	end

	def signin
		@user = User.new
		@usergoals = Goal.new
	end

	def passwordchange
		cpuser = User.find_by(id: current_user.id)
				if cpuser.update_attribute(:password, params[:user][:password])
			flash[:changepassword] = 'Your Password has changed successfully'
			redirect_to action:'main', :id => current_user.id
		else
			redirect_to action:'main', :id => current_user.id
		end
	end

	def authenticateuser
		@user = User.find_by(emailid: params[:user][:emailid].downcase)
		if @user && @user.authenticate(params[:user][:password])
	      	log_in @user
	      	
		    redirect_to action: 'main', :id => current_user.id
	    else
	      flash[:notice] = 'Unknown user. Please check your username and password.'
	      redirect_to :action =>'signin'
	    end
	end

	def updategoal
		# @goal = Goal.new()
		
		# @goal = Goal.create({:label => params[:label],:goal => params[:goal],:status => params[:status],:remarks => params[:remarks],:userid => current_user.id});
		usergoal = Goal.find_by(id: params[:goalid])
		if usergoal.update_attributes(:label => params[:label],:goal => params[:goal],:status => params[:status],:remarks => params[:remarks],:userid => current_user.id)
			redirect_to action: 'main', :id => current_user.id
		else
			redirect_to action: 'main', :id => current_user.id
		end
	end

	def addusergoal
		@goal = Goal.new()
		
		@goal = Goal.create({:label => '',:goal => '',:status => 'New',:remarks => '',:userid => current_user.id,:DoToday => Time.now});
		if @goal.save
			redirect_to action: 'main', :id => current_user.id
		else
			redirect_to action: 'main', :id => current_user.id
		end
	end

	# rendering html data to view through ajax data type html
	def getallpendinggoals

		if(params[:timespan] == "All")

	#	@usersallpendinggoals = Goal.where(:userid => current_user.id).where.not(:status => "Done").select([:id, :goal, :status, :remarks, :created_at])

		@usersallpendinggoals = Goal.where("userid = :userid and Date(DoToday) != :todaydate", { userid: current_user.id, todaydate: Date.today }).where.not(:status => "Done").select([:id, :goal, :status, :remarks, :created_at])

		render :layout => false


		else if(params[:timespan] == "Yesterday")

		@usersallpendinggoals = Goal.where("userid = :userid and Date(created_at) = :date and Date(DoToday) != :todaydate", { userid: current_user.id, date: Date.today - 1.day, todaydate: Date.today }).where.not(:status => "Done").select([:id, :goal, :status, :remarks, :created_at])

		render :layout => false

		else if(params[:timespan] == "Last week")

		@usersallpendinggoals = Goal.where("userid = :userid and Date(DoToday) != :todaydate and Date(created_at) BETWEEN :fromdate AND :todate", { userid: current_user.id, todaydate: Date.today, fromdate: Date.today - 7.days, todate:  Date.today - 1.day}).where.not(:status => "Done").select([:id, :goal, :status, :remarks, :created_at])

		render :layout => false

		else if(params[:timespan] == "Last One Month")

		@usersallpendinggoals = Goal.where("userid = :userid and Date(DoToday) != :todaydate and Date(created_at) BETWEEN :fromdate AND :todate", { userid: current_user.id, todaydate: Date.today, fromdate: Date.today - 1.month, todate:  Date.today - 1.day}).where.not(:status => "Done").select([:id, :goal, :status, :remarks, :created_at])

		render :layout => false

		else if(params[:timespan] == "Last Three Months")

		@usersallpendinggoals = Goal.where("userid = :userid and Date(DoToday) != :todaydate and Date(created_at) BETWEEN :fromdate AND :todate", { userid: current_user.id, todaydate: Date.today, fromdate: Date.today - 3.months, todate:  Date.today - 1.day}).where.not(:status => "Done").select([:id, :goal, :status, :remarks, :created_at])

		render :layout => false

		else if(params[:timespan] == "Last Six Months")

		@usersallpendinggoals = Goal.where("userid = :userid and Date(DoToday) != :todaydate and Date(created_at) BETWEEN :fromdate AND :todate", { userid: current_user.id, todaydate: Date.today, fromdate: Date.today - 6.months, todate:  Date.today - 1.day}).where.not(:status => "Done").select([:id, :goal, :status, :remarks, :created_at])

		render :layout => false

		end

		end

		end

		end

		end

		end

	#	respond_to do |format|
	#    	format.html
	#    	format.json {render :json => @usersallpendinggoals}
	#	end

	#	redirect_to action: 'main', :id => current_user.id

  	end

  	def setgoaltodotoday

  		usergoal = Goal.find_by(id: params[:goalid])
		if usergoal.update_attributes(:DoToday => Time.now)

			@usergoalsdotoday = Goal.where("userid = :userid and Date(DoToday) = :date", { userid: current_user.id, date: Date.today })

			respond_to do |format|
			   	format.html
			   	format.json {render :json => @usergoalsdotoday}
			end
		
		end

  	end

	def logout
		log_out
		flash[:logout] = 'You have been logged out successfully'
		redirect_to root_url
	end

	def reg_params
		params.require(:user).permit(:name, :emailid, :password, :password_confirmation)
	end
  

end
