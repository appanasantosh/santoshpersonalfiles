class AddAttachmentTomailboxernotifications < ActiveRecord::Migration
  def self.up
    change_table :mailboxer_notifications do |t|
      t.attachment :document
    end
  end

  def self.down
    remove_attachment :mailboxer_notifications, :document
  end
end
