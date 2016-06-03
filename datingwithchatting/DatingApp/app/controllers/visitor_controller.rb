class VisitorController < ApplicationController
	before_action  only: [:create, :destroy, :authenticate]

	def create

		@visitor = Visitor.create(reg_params)

		if @visitor.save
			flash[:success] = "Registered successfully!"
			redirect_to contestdetails_path
		else
			flash[:error] = "Not registered!"
			redirect_to contestdetails_path
		end
	end

	def authenticate
		visitor = Visitor.find_by(email: params[:visitor][:email].downcase)
		if visitor && visitor.authenticate(params[:visitor][:password])

			flash[:success] = "Signin successful!"
			redirect_to :controller => "contest", :action => "contestpage", id: visitor.id

		else
			flash[:error] = "Signin failed!"
			redirect_to contestdetails_path
		end
	end

	def destroy
	end

	private

	def reg_params
		params.require(:visitor).permit(:name, :email, :password, :password_confirmation)
	end

end
