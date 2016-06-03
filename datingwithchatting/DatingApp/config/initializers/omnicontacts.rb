 require 'omnicontacts'

    Rails.application.middleware.use OmniContacts::Builder do
        importer :gmail, "383465264657-a58oqfi1vfp4t9sifhpsjad4si14bvvu.apps.googleusercontent.com", "6uoLJTJU5R7Ywcfm4OX8LkS-", { :max_results => 1000,:ssl_ca_path => "/etc/ssl/certs/curl-ca-bundle.crt",:approval_prompt => 'auto'}
        importer :yahoo, "dj0yJmk9WE1Odkc4MlIwOGVMJmQ9WVdrOWNFcDBkMnA0TkdrbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD02ZQ--","a2705cf584623e30d80be3a9a68addb54f73d07d", {:callback_path => '/callback'}
    end
