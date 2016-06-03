class CreateContestResults < ActiveRecord::Migration
  def change
    create_table :contest_results do |t|
      t.integer :Contest_id
      t.integer :Visitor_id
      t.integer :User_id

      t.timestamps null: false
    end
  end
end
