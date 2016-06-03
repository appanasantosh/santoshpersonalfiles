class CreateContests < ActiveRecord::Migration
  def change
    create_table :contests do |t|
      t.string :name
      t.string :description
      t.datetime :publishdate
      t.datetime :lastdate
      t.datetime :contestdate

      t.timestamps null: false
    end
  end
end
