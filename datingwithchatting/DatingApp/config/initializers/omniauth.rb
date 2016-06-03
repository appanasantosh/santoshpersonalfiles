Rails.application.config.middleware.use OmniAuth::Builder do
  provider :facebook, '146761285676324', 'f4981cf2eeffc4d839baf25c4c6778c4'
  provider :google_oauth2, '383465264657-a58oqfi1vfp4t9sifhpsjad4si14bvvu.apps.googleusercontent.com', '6uoLJTJU5R7Ywcfm4OX8LkS-'
  provider :twitter, 'EnFB1kT4uHrtHv7ydEcqQqJFE','cNKAQzLd3d0GTHY7rluJBCS3OtovLKknJAdUt4688oVS2s76J6'
end