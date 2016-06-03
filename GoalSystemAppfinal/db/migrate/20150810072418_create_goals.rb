class CreateGoals < ActiveRecord::Migration
  def change
    create_table :goals do |t|
      t.string :label
      t.string :goal
      t.string :status
      t.string :remarks
      t.integer :userid

      t.timestamps null: false
    end
  end
end
