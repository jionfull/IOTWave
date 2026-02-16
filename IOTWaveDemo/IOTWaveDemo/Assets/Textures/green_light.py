from PIL import Image, ImageDraw

"""
绿灯纹理生成脚本
独立运行生成绿灯纹理
"""

def create_green_light_texture():
    """创建绿灯纹理"""
    # 创建16x16的黑色背景图像
    img = Image.new("RGB", (16, 16), color="#000000")
    draw = ImageDraw.Draw(img)
    
    # 绿色填充整个图像
    draw.rectangle([0, 0, 16, 16], fill="#00FF00")
    
    return img

def main():
    """主函数"""
    print("生成绿灯纹理...")
    img = create_green_light_texture()
    img.save("green_light.png")
    print("绿灯纹理已生成: green_light.png")

if __name__ == "__main__":
    main()