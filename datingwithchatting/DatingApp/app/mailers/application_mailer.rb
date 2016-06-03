class ApplicationMailer < ActionMailer::Base
include UserHelper
  default from: "from@example.com"
  layout 'mailer'
end
