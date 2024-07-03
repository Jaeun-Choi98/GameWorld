package models

import (
	"database/sql/driver"
	"encoding/json"
)

type User struct {
	UserId   uint   `gorm:"column:user_id" json:"userId"`
	Email    string `gorm:"column:email" json:"email"`
	Password string `gorm:"column:passwd" json:"passwd"`
}

type Player struct {
	UserId     uint `gorm:"column:user_id" json:"userId"`
	PlyerId    uint `gorm:"column:player_id" json:"playerId"`
	PlayerInfo Info `gorm:"column:info;foreignKey:Name;references:player_id" json:"playerInfo"`
}

type Info struct {
	Name      string
	Money     int
	Speed     int
	JumpPower int
}

// Value 메서드 구현: Info 구조체를 JSON으로 변환
func (p Info) Value() (driver.Value, error) {
	return json.Marshal(p)
}

// Scan 메서드 구현: JSON 데이터를 Info 구조체로 변환
func (p *Info) Scan(value interface{}) error {
	bytes, _ := value.([]byte)
	return json.Unmarshal(bytes, p)
}
