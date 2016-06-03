class ContestController < ApplicationController
    before_action :signed_in_user, only: [:create, :destroy]

    def create

        @contest = Contest.create(contest_params)

        if @contest.save
            flash[:success] = "Contest created!"
            redirect_to contestdetails_path
        else
            flash[:error] = "Comment not posted!"
            redirect_to contestdetails_path
        end
    end

    def gotocontest
        redirect_to contestdetails_path
    end


    def contest_result
        @contest = Contest.find_by_name("Photo contest")
        @contestresult = ContestResult.where(Contest_id: @contest.id)
        @maleids, @femaleids = [], []

        @contestresult.each do |cr|
            @maleids << cr.User_id if cr.user.gender == 'male'
            @femaleids << cr.User_id if cr.user.gender == 'female'
        end

        if !@maleids.nil?

            @contestresultForMale = @maleids.group_by{|a| a }.sort_by{|a,b| b.size<=>a.size}.first[0]
            
        end

        if !@femaleids.nil?

            @contestresultForFemale = @femaleids.group_by{|a| a }.sort_by{|a,b| b.size<=>a.size}.first[0]
            
        end

    end





    def contest_done
        render :layout => false
    end

    def plancontest
        @contest = Contest.new
        @users = User.paginate(page: params[:page], per_page: 1)
    end

    def insertselectedids
        @contest = Contest.find_by_name("Photo contest")
        @contestresult = ContestResult.new
        @contestresult.Contest_id = @contest.id
        @contestresult.Visitor_id = params[:vid]
        @contestresult.User_id = params[:mid]
        #@contestresult = ContestResult.create(contest_result_params)

        if @contestresult.save
            @contestresult = ContestResult.new
            @contestresult.Contest_id = @contest.id
            @contestresult.Visitor_id = params[:vid]
            @contestresult.User_id = params[:fid]
            #@contestresult = ContestResult.create(contest_result_params)

            if @contestresult.save
            
                redirect_to action: 'contest_done'

            else
                
                respond_to do |format|
                
                    format.json { render :text => "Your voting not done"}
                end
                
            end

        else
            
            respond_to do |format|
                 
                format.json { render :text => "Your voting not done"}
            end
            
        end

    end

    def contestpage
        @contest = Contest.find_by_name("Photo contest")
        @uid = params[:id]
        @vid = ContestResult.find_by_Visitor_id(@uid)
        @selectedpics = PictureSelection.all
    end

    def destroy
    end

    private

    def contest_params
        params.require(:contest).permit(:name, :description, :publishdate, :lastdate, :contestdate)
    end

    def contest_result_params
        params.require(:contestresult).permit(:Visitor_id, :User_id, :Contest_id)
    end

end
