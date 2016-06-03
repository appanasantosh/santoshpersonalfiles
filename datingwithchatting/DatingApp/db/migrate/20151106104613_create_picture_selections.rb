class CreatePictureSelections < ActiveRecord::Migration
  def change
    create_table :picture_selections do |t|
      t.integer :user_id

      t.timestamps null: false
    end
  end
end
