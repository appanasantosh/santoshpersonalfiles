require 'spec_helper'

describe "StaticPages" do
  subject {page}
  describe "Home page" do
    before {visit root_path}
    it {should have_title(full_title('Home'))}

      # Run the generator again with the --webrat flag if you want to use webrat methods/matchers
      
    end
    

   describe "Help page" do
    before {visit help_path}
   	it {should have_title(full_title('Help'))}
  
   	
   end


  describe "AboutUs page" do
    before {visit aboutus_path}
  	it {should have_title(full_title('AboutUs'))}
   
  	
  end
	

  describe "Contact page" do
    before {visit contact_path}
    it {should have_title(full_title('Contact'))}
   
    
  end
  it "should have the right links on the layout" do
    visit root_path
    click_link "AboutUs"
    expect(page).to have_title(full_title('AboutUs'))
    click_link "Help"
    expect(page).to have_title(full_title('Help'))
    click_link "Contact"
    expect(page).to have_title(full_title('Contact'))
  end
  
end
