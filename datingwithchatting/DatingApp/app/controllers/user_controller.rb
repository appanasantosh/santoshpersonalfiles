class UserController < ApplicationController
  # after_action :onlineuser ,only:[ :show]
  before_action :onlineuser ,only:[:destroy]
  before_action :signed_in_user, only:[:profile, :profileupdate, :picsandvideos,:showpics]
 
  def new
    @user = User.new
  end

  def create
    @user = User.create(reg_params)
    if @user.save
      
      sign_in @user
      onlineuser
  
      Datingappmailer.sendmail(@user, 0,"","").deliver
      flash[:success]= "Welcome to the Dating App"
      redirect_to action: 'show' ,id: @user.id
    else
      render 'new'
    end
  end

  def social_authentication
    auth_hash = request.env['omniauth.auth']
    userfacebookmail =""
    message=""

   @authorization = Authorization.find_by_provider_and_uid(auth_hash["provider"], auth_hash["uid"])
    if @authorization
        # render :text => "Welcome back #{@authorization.user.name}! You have already signed up."
        user = User.find_by(id: @authorization.user_id)
        sign_in user
        redirect_to action: 'show', id: user.id
    else
      if auth_hash["provider"] == "google_oauth2"
       userfacebookmail = auth_hash[:info][:email]
       message = "You are registered. Signin with your google account"
     else if auth_hash["provider"] == "facebook"
       userfacebookmail = auth_hash[:info][:name] +'@'+'facebook.com'
       message = "You are registered. Signin with your facebook account"
     else if auth_hash["provider"] == "twitter"
        userfacebookmail = auth_hash[:info][:name] +'@'+'twitter.com'
       message = "You are registered. Signin with your twitter account"
     end
     end  
     end
       @user = User.new(:name => auth_hash[:info][:name], :email => userfacebookmail, :password => userfacebookmail)
       @user.authorizations.build :provider => auth_hash["provider"], :uid => auth_hash["uid"]
       @user.save
       sign_in @user
       flash[:message] = message
         # redirect_to @user
        
       redirect_to action: 'show', id: @user.id
    end

 
    

  end

def importcontacts
 
    @contacts = request.env['omnicontacts.contacts']             # getting the contacts from gmail after the successfull authentication
    @importedcontacts = []                                       # created array 
    @contacts.each do |contact|                  
      @importedcontacts.push({name: contact[:name], email: contact[:email]})    #with in loop pushing hashes to array      
    end
     @importedcontacts= @importedcontacts.sort_by{|cnt| cnt[:name]}           # sorting the array of hashes based on the key:name 
 end

def sendinvitation
  @currentusername = current_user.name 
  Datingappmailer.sendmail(@currentusername, 1,params[:name],params[:email]).deliver
  @message= "invitation sent successfully"
  
  respond_to do |format|
    format.json {render text: "invitation sent successfully"}
  end
end

  def profile
    @user = current_user 
  end

  def profileupdate
    @user = User.find(current_user.id)
    if @user.update_attributes(profile_params)
      flash[:success] = "Profile updated"
      redirect_to action:'show', id: @user.id
    else
      render 'profile'
    end
  end


  def authenticate
    user = User.find_by(email: params[:user][:email].downcase)
    @signinuser = params[:signinway]
    if user && user.authenticate(params[:user][:password])
      sign_in user
      onlineuser
      if @signinuser == "appsignin" 
        redirect_to action: 'show', id: user.id
      else if @signinuser == "visitorsignin"
        redirect_to :controller => "contest", :action => "contestpage", id: user.id
      end
      end
    else
      flash.now[:error] = "Invalid emailid or password"
      render 'signin'
    end
  end

  def picsandvideos
    @user = User.find(current_user.id)
    @profileimage = @user.profileimages
  end


  def showpics
    @user = User.find(params[:userid])
    @profileimage = @user.profileimages

  end



  def uploadimages
    @user = User.find(current_user.id)
    @profileimage = @user.profileimages.build(img_params)
    if @profileimage.save
      redirect_to action: 'picsandvideos'
    else
      redirect_to action: 'picsandvideos'
    end
  end

  def signin
    @signinway = params[:signinway]
    
  end

  def show
    @user = User.find(params[:id])
  end

  def destroy
    sign_out
    redirect_to root_url
  end

  def usersprofiles
    @users = User.paginate(page: params[:page], per_page: 2)

  end

  def signed_in_user
      unless signed_in?
        store_location
        redirect_to signin_url, notice: "Please sign in."
      end
  end
  
  def chat
    @users = User.where.not(id: current_user.id)
  end


  private

  def reg_params
    params.require(:user).permit(:name, :email, :password, :password_confirmation)
  end

  def profile_params
    params.require(:user).permit(:name, :email,:dateofbirth,:gender,:current_status,:qualification,:country,:userimage,:occupation, :password, :password_confirmation)

  end

  def img_params
    params.require(:profileimage).permit(:image)
  end

  def onlineuser
    current_user.update_attribute(:last_seen_at, Time.now)
  end




  
end
