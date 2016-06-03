class CreateProfileimages < ActiveRecord::Migration
  def change
    create_table :profileimages do |t|
      t.integer :user_id
      t.attachment :image

      t.timestamps null: false
    end
  end
end
