class CreateContestDetails < ActiveRecord::Migration
  def change
    create_table :contest_details do |t|
      t.integer :contest_id
      t.integer :visitor_id
      t.integer :profileimage_id

      t.timestamps null: false
    end
  end
end
