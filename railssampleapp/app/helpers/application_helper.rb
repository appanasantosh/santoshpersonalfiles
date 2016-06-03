module ApplicationHelper
	def full_title(pagetitle)
		basetitle= "Ruby on Rails Tutorial Sample App"
		if pagetitle.empty?
			basetitle
		else 
		    basetitle+" | "+pagetitle
		end			
	end
end
